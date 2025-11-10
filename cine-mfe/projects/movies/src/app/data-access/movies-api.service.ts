import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import {
  loadMoviesCache,
  persistMoviesCache,
  loadSession,
} from '@shared/auth/auth-storage';

export interface MovieDto {
  id?: number;
  titulo: string;
  descripcion?: string;
  genero?: string;
  director?: string;
  anio?: number;
}

export type MoviePayload = Omit<MovieDto, 'id'>;

export interface MovieMutationResponse {
  success?: boolean;
  message?: string;
  idPelicula?: number;
}

interface MoviesEnvelope {
  success?: boolean;
  count?: number;
  items?: BackendMovieDto[];
  data?: BackendMovieDto[];
}

interface BackendMovieDto extends MovieDto {
  idPelicula?: number;
  fechaCreacion?: string;
}

@Injectable({
  providedIn: 'root',
})
export class MoviesApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:8080/api/v1';

  list(): Observable<MovieDto[]> {
    return this.http
      .get<MoviesEnvelope | BackendMovieDto[]>(`${this.apiBase}/movies`)
      .pipe(
        map((response) => this.mapMoviesResponse(response)),
        tap((movies) => this.cacheMovies(movies)),
      );
  }

  create(movie: MoviePayload): Observable<MovieMutationResponse> {
    const body = {
      ...this.mapToBackendPayload(movie),
      usuarioCreacion: this.resolveAdminIdentifier(),
    };
    return this.http.post<MovieMutationResponse>(`${this.apiBase}/movie`, body);
  }

  remove(id: number): Observable<MovieMutationResponse> {
    return this.http.delete<MovieMutationResponse>(`${this.apiBase}/movie/${id}`);
  }

  getCachedMovies(): MovieDto[] | null {
    const snapshot = loadMoviesCache<MovieDto>();
    if (!snapshot || !Array.isArray(snapshot.items)) {
      return null;
    }
    return snapshot.items;
  }

  private mapMoviesResponse(
    response: MoviesEnvelope | BackendMovieDto[],
  ): MovieDto[] {
    const collection = Array.isArray(response)
      ? response
      : response.items ?? response.data ?? [];
    return collection.map((movie) => this.normalizeMovie(movie));
  }

  private cacheMovies(movies: MovieDto[]): void {
    persistMoviesCache<MovieDto>(movies);
  }

  private mapToBackendPayload(movie: MoviePayload): Record<string, unknown> {
    return {
      titulo: movie.titulo,
      descripcion: movie.descripcion,
      genero: movie.genero,
      director: movie.director,
      anio:
        typeof movie.anio === 'number'
          ? movie.anio
          : this.parseYear(movie.anio as unknown as string),
    };
  }

  private parseYear(value?: string | number | null): number | undefined {
    if (value == null) {
      return undefined;
    }
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value;
    }
    if (typeof value === 'string' && value.trim().length) {
      const numeric = Number(value.trim());
      if (Number.isFinite(numeric)) {
        return numeric;
      }
    }
    return undefined;
  }

  private resolveAdminIdentifier(): string | undefined {
    const session = loadSession();
    const user = session.user;
    if (!user) {
      return undefined;
    }
    const candidates = [
      user.usuario,
      user.usuarioCreacion,
      user.fullName,
      user.correo,
    ];
    for (const candidate of candidates) {
      if (typeof candidate === 'string' && candidate.trim().length > 0) {
        return candidate.trim();
      }
    }
    return undefined;
  }

  private normalizeMovie(movie: BackendMovieDto | MovieDto): MovieDto {
    const backendMovie = movie as BackendMovieDto;
    const anio =
      movie.anio ??
      backendMovie.anio ??
      this.parseYear(backendMovie.fechaCreacion);
    return {
      id: movie.id ?? backendMovie.idPelicula,
      titulo: movie.titulo,
      descripcion: movie.descripcion,
      genero: movie.genero,
      director: movie.director,
      anio,
    };
  }
}
