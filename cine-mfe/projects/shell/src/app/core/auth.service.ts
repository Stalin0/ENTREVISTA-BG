import { inject, Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { clearSession, loadSession, persistSession, StoredUser } from '@shared/auth/auth-storage';

interface LoginRequest {
  correo: string;
  contrasenia: string;
}

interface LoginResponse {
  token?: string;
  jwt?: string;
  accessToken?: string;
  access_token?: string;
  usuario?: StoredUser & { rol?: string; idRol?: number };
  user?: StoredUser & { rol?: string; idRol?: number };
  data?: StoredUser & { rol?: string; idRol?: number };
  [key: string]: unknown;
}

export interface RegisterRequest {
  fullName: string;
  correo: string;
  contrasenia: string;
  usuario?: string;
  idRol?: number;
}

function resolveToken(response: LoginResponse): string {
  const token =
    response.token ??
    response.jwt ??
    response.accessToken ??
    response.access_token;
  if (typeof token === 'string' && token.length > 0) {
    return token;
  }
  throw new Error('Credenciales Incorrectas o Comuniquese con su administrador para mayor información.');
}

function resolveUser(response: LoginResponse): StoredUser {
  const candidate = (response.usuario ??
    response.user ??
    response.data ??
    {}) as StoredUser & {
    rol?: string;
    roleName?: string;
    nombreRol?: string;
  };
  const role =
    candidate.role ??
    normalizeRole(candidate.rol) ??
    normalizeRole(candidate.roleName) ??
    normalizeRole(candidate.nombreRol) ??
    (typeof candidate.idRol === 'number'
      ? candidate.idRol === 1
        ? 'ADMIN'
        : 'INVITED'
      : undefined) ??
    'INVITED';

  return {
    ...candidate,
    role,
  };
}

function normalizeRole(value: unknown): 'ADMIN' | 'INVITED' | undefined {
  if (typeof value !== 'string') {
    return undefined;
  }
  const upper = value.toUpperCase();
  if (upper.includes('ADMIN')) {
    return 'ADMIN';
  }
  if (upper.includes('INV') || upper.includes('GUEST')) {
    return 'INVITED';
  }
  return undefined;
}

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  const [, rawPayload] = token.split('.');
  if (!rawPayload) {
    return null;
  }
  try {
    const normalized = rawPayload.replace(/-/g, '+').replace(/_/g, '/');
    const padded =
      normalized + '='.repeat((4 - (normalized.length % 4 || 4)) % 4);
    const json = decodeBase64(padded);
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

type BufferLike = {
  from(input: string, encoding: string): { toString(encoding: string): string };
};

function decodeBase64(payload: string): string {
  if (typeof atob === 'function') {
    return decodeURIComponent(
      atob(payload)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join(''),
    );
  }
  const globalBuffer = (globalThis as { Buffer?: BufferLike }).Buffer;
  if (globalBuffer) {
    return globalBuffer.from(payload, 'base64').toString('utf-8');
  }
  throw new Error('No base64 decoder available in this environment.');
}

function resolveRoleFromClaims(
  claims: Record<string, unknown>,
): 'ADMIN' | 'INVITED' | undefined {
  const candidateKeys = [
    'role',
    'roles',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  ];

  for (const key of candidateKeys) {
    const value = claims[key];
    if (typeof value === 'string') {
      const normalized = normalizeRole(value);
      if (normalized) {
        return normalized;
      }
    }
    if (Array.isArray(value)) {
      for (const entry of value) {
        if (typeof entry === 'string') {
          const normalized = normalizeRole(entry);
          if (normalized) {
            return normalized;
          }
        }
      }
    }
  }
  return undefined;
}

function enrichUserWithTokenClaims(token: string, user: StoredUser): StoredUser {
  const claims = decodeJwtPayload(token);
  if (!claims) {
    return user;
  }

  const role = resolveRoleFromClaims(claims) ?? user.role;
  const fullName = user.fullName ?? (claims['name'] as string | undefined);
  const correo =
    user.correo ??
    (claims['email'] as string | undefined) ??
    (claims['sub'] as string | undefined);
  const idRol =
    user.idRol ?? (role ? (role === 'ADMIN' ? 1 : 2) : undefined);
  const userNameFromUser =
    typeof user.usuario === 'string' ? user.usuario.trim() : undefined;
  const userNameFromClaims =
    typeof claims['usuario'] === 'string'
      ? (claims['usuario'] as string).trim()
      : undefined;
  const usuario = userNameFromUser || userNameFromClaims;

  return {
    ...user,
    role,
    fullName,
    correo,
    idRol,
    ...(usuario ? { usuario } : {}),
  };
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:8080/api/v1';

  private readonly tokenState = signal<string | null>(loadSession().token);
  private readonly userState = signal<StoredUser | null>(loadSession().user);

  readonly token = computed(() => this.tokenState());
  readonly user = computed(() => this.userState());
  readonly isLoggedIn = computed(() => !!this.tokenState());
  readonly role = computed<'ADMIN' | 'INVITED' | null>(() => {
    const user = this.userState();
    if (!user) {
      return null;
    }
    if (user.role) {
      return user.role;
    }
    if (typeof user.idRol === 'number') {
      return user.idRol === 1 ? 'ADMIN' : 'INVITED';
    }
    return null;
  });
  readonly isAdmin = computed(() => this.role() === 'ADMIN');
  readonly isInvited = computed(() => this.role() === 'INVITED');

  login(payload: LoginRequest): Observable<StoredUser> {
    return this.http
      .post<LoginResponse>(`${this.apiBase}/login`, payload)
      .pipe(
        tap((response) => {
          const token = resolveToken(response);
          const user = resolveUser(response);
          const enrichedUser = enrichUserWithTokenClaims(token, user);
          this.persistSession(token, enrichedUser);
        }),
        map(() => this.userState() as StoredUser),
      );
  }

  registerAdmin(payload: RegisterRequest) {
    const body = {
      ...payload,
      usuario: payload.usuario ?? payload.correo,
      idRol: payload.idRol ?? 1,
    };
    return this.registerUser(body);
  }

  registerInvite(payload: RegisterRequest) {
    const usuario = this.userState()?.usuario ?? payload.usuario ?? '';
    const body = {
      ...payload,
      usuario: usuario || payload.usuario || payload.usuario,
      idRol: 2,
    };
    console.log("Invitado", this.registerInvite)
    return this.registerUser(body);
  }

  logout(): void {
    clearSession();
    this.tokenState.set(null);
    this.userState.set(null);
  }

  private registerUser(body: RegisterRequest) {
    return this.http.post(`${this.apiBase}/user`, body);
  }

  private persistSession(token: string, user: StoredUser) {
    persistSession(token, user);
    this.tokenState.set(token);
    this.userState.set(user);
  }
}





