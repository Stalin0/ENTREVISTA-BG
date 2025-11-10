import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  MovieDto,
  MoviePayload,
  MoviesApiService,
  MovieMutationResponse,
} from '../../data-access/movies-api.service';
import { HttpErrorResponse } from '@angular/common/http';
import { loadSession } from '@shared/auth/auth-storage';

@Component({
  selector: 'app-movies-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './movies-dashboard.component.html',
  styleUrl: './movies-dashboard.component.scss',
})
export class MoviesDashboardComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly moviesApi = inject(MoviesApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly form = this.fb.nonNullable.group({
    titulo: ['', [Validators.required, Validators.minLength(2)]],
    director: ['', [Validators.required, Validators.minLength(3)]],
    genero: ['', [Validators.required, Validators.minLength(2)]],
    anio: [
      new Date().getFullYear(),
      [
        Validators.required,
        Validators.min(1900),
        Validators.max(new Date().getFullYear() + 1),
      ],
    ],
    descripcion: ['', [Validators.required, Validators.minLength(3)]],
  });

  readonly movies = signal<MovieDto[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly feedback = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  readonly session = signal(loadSession());
  readonly isAdmin = computed(() => {
    const user = this.session().user;
    if (!user) {
      return false;
    }
    if (user.role) {
      return user.role === 'ADMIN';
    }
    if (typeof user.idRol === 'number') {
      return user.idRol === 1;
    }
    return false;
  });

  ngOnInit(): void {
    this.fetchMovies();
  }

  fetchMovies(options?: { forceRefresh?: boolean }): void {
    const forceRefresh = options?.forceRefresh ?? false;
    if (!forceRefresh) {
      const cached = this.moviesApi.getCachedMovies();
      if (cached && cached.length > 0) {
        this.movies.set(cached);
        this.loading.set(false);
        return;
      }
    }

    this.loading.set(true);
    this.error.set(null);
    this.moviesApi
      .list()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (movies) => this.movies.set(movies ?? []),
        error: (err: unknown) => this.error.set(this.resolveError(err)),
        complete: () => this.loading.set(false),
      });
  }

  resetForm(): void {
    this.form.reset({
      titulo: '',
      director: '',
      genero: '',
      anio: new Date().getFullYear(),
      descripcion: '',
    });
    this.form.markAsPristine();
    this.form.markAsUntouched();
  }

  save(): void {
    if (!this.isAdmin()) {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    this.feedback.set(null);

    const payload = this.form.getRawValue() as MoviePayload;
    this.moviesApi
      .create(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: MovieMutationResponse | void) => {
          this.feedback.set(
            (response as MovieMutationResponse)?.message ??
              'Pelicula registrada.',
          );
          this.resetForm();
          this.fetchMovies({ forceRefresh: true });
        },
        error: (err: unknown) => this.error.set(this.resolveError(err)),
        complete: () => this.saving.set(false),
      });
  }

  remove(movie: MovieDto): void {
    if (!this.isAdmin() || !movie.id) {
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    this.moviesApi
      .remove(movie.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.feedback.set(
            response?.message ?? `Se elimino ${movie.titulo}.`,
          );
          this.fetchMovies({ forceRefresh: true });
        },
        error: (err: unknown) => this.error.set(this.resolveError(err)),
        complete: () => this.saving.set(false),
      });
  }

  private resolveError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error === 'string' && error.error.length) {
        return error.error;
      }
      if (error.error?.message) {
        return error.error.message;
      }
      return `Error ${error.status || ''} ${error.statusText || ''}`.trim();
    }
    if (error instanceof Error) {
      return error.message;
    }
    return 'No fue posible completar la operacion solicitada.';
  }

  controlInvalid(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  controlErrorMessage(controlName: keyof typeof this.form.controls): string | null {
    const control = this.form.controls[controlName];
    if (!control || !(control.dirty || control.touched)) {
      return null;
    }

    if (control.hasError('required')) {
      return 'Este campo es obligatorio.';
    }
    if (control.hasError('minlength')) {
      const requiredLength = control.getError('minlength')?.requiredLength;
      return `Debe tener al menos ${requiredLength} caracteres.`;
    }
    if (control.hasError('min') || control.hasError('max')) {
      return 'Ingresa un año válido.';
    }

    return null;
  }
}
