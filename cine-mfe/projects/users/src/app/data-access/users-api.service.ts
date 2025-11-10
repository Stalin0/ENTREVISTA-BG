import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { loadSession, StoredUser } from '@shared/auth/auth-storage';

export interface UserDto extends StoredUser {
  id?: number;
}

export interface InvitePayload {
  fullName: string;
  correo: string;
  contrasenia: string;
  usuarioCreacion?: string;
}

export interface UpdateInvitePayload {
  fullName: string;
  correo: string;
  activo: boolean;
}

export interface ApiMessageResponse {
  success?: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UsersApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:8080/api/v1';

  createInvite(payload: InvitePayload): Observable<UserDto> {
    const sessionUser = loadSession().user;
    const normalize = (value?: string) =>
      typeof value === 'string' ? value.trim() : '';

    const payloadCreator = normalize(payload.usuarioCreacion);
    const adminUsuario = normalize(sessionUser?.usuario);
    const adminStoredCreator = normalize(sessionUser?.usuarioCreacion);
    const adminFullName = normalize(sessionUser?.fullName);
    const adminCorreo = normalize(sessionUser?.correo);
    const usuarioCreacion =
      payloadCreator ||
      adminUsuario ||
      adminStoredCreator ||
      adminFullName ||
      adminCorreo ||
      normalize(payload.correo);
    const body = {
      ...payload,
      idRol: 2,
      usuarioCreacion,
    };

    return this.http.post<UserDto>(`${this.apiBase}/user`, body);
  }

  deleteUser(userId: number): Observable<ApiMessageResponse> {
    return this.http.delete<ApiMessageResponse>(`${this.apiBase}/user/${userId}`);
  }

  updateUser(userId: number, payload: UpdateInvitePayload): Observable<ApiMessageResponse> {
    return this.http.put<ApiMessageResponse>(`${this.apiBase}/user/${userId}`, payload);
  }

  listUsers(): Observable<UserDto[]> {
    return this.http
      .get<unknown>(`${this.apiBase}/user`)
      .pipe(map(normalizeUsersResponse));
  }
}

function normalizeUsersResponse(payload: unknown): UserDto[] {
  const rawList = extractList(payload);
  return rawList.map((entry) => normalizeUserEntry(entry));
}

function extractList(payload: unknown): unknown[] {
  if (Array.isArray(payload)) {
    return payload;
  }
  if (payload && typeof payload === 'object') {
    const candidates = [
      (payload as { data?: unknown }).data,
      (payload as { usuarios?: unknown }).usuarios,
      (payload as { items?: unknown }).items,
    ];
    for (const candidate of candidates) {
      if (Array.isArray(candidate)) {
        return candidate;
      }
    }
    return [payload];
  }
  return [];
}

function normalizeUserEntry(entry: unknown): UserDto {
  if (!entry || typeof entry !== 'object') {
    return { id: undefined } as UserDto;
  }
  const record = entry as Record<string, unknown>;
  const id =
    getNumber(record, 'id') ??
    getNumber(record, 'idUsuario') ??
    getNumber(record, 'id_user') ??
    undefined;

  const fullName =
    (record['fullName'] as string | undefined) ??
    (record['nombreCompleto'] as string | undefined) ??
    (record['nombre'] as string | undefined);

  const activo =
    record['activo'] ??
    record['estado'] ??
    record['estatus'] ??
    record['status'];

  return {
    ...(entry as UserDto),
    ...(typeof id === 'number' ? { id } : {}),
    ...(fullName ? { fullName } : {}),
    ...(typeof activo !== 'undefined' ? { activo } : {}),
  };
}

function getNumber(
  source: Record<string, unknown>,
  key: string,
): number | undefined {
  const value = source[key];
  if (typeof value === 'number' && Number.isFinite(value)) {
    return value;
  }
  if (typeof value === 'string' && value.trim().length) {
    const parsed = Number(value.trim());
    return Number.isFinite(parsed) ? parsed : undefined;
  }
  return undefined;
}
