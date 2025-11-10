export const AUTH_TOKEN_KEY = 'cine.auth.token';
export const AUTH_USER_KEY = 'cine.auth.user';
export const MOVIES_CACHE_KEY = 'cine.movies.cache';

export interface StoredUser {
  id?: number;
  idRol?: number;
  fullName?: string;
  correo?: string;
  role?: 'ADMIN' | 'INVITED';
  usuario?: string;
  usuarioCreacion?: string;
  [key: string]: unknown;
}

export interface StoredSession {
  token: string | null;
  user: StoredUser | null;
}

export interface MoviesCacheSnapshot<T = unknown> {
  items: T[];
  timestamp: number;
}

function ensureAdminCreator(user: StoredUser | null | undefined): void {
  if (!user) {
    return;
  }
  const isAdmin =
    user.role === 'ADMIN' ||
    (typeof user.idRol === 'number' && user.idRol === 1);
  if (!isAdmin) {
    return;
  }

  const normalizedCreator =
    typeof user.usuarioCreacion === 'string'
      ? user.usuarioCreacion.trim()
      : '';
  if (normalizedCreator) {
    user.usuarioCreacion = normalizedCreator;
    return;
  }

  const usuario =
    typeof user.usuario === 'string' ? user.usuario.trim() : '';
  if (usuario) {
    user.usuarioCreacion = usuario;
    return;
  }

  const fullName =
    typeof user.fullName === 'string' ? user.fullName.trim() : '';
  if (fullName) {
    user.usuarioCreacion = fullName;
    return;
  }

  const correo =
    typeof user.correo === 'string' ? user.correo.trim() : '';
  if (correo) {
    user.usuarioCreacion = correo;
  }
}

export function isBrowser(): boolean {
  return typeof window !== 'undefined' && typeof localStorage !== 'undefined';
}

export function loadSession(): StoredSession {
  if (!isBrowser()) {
    return { token: null, user: null };
  }

  const rawToken = localStorage.getItem(AUTH_TOKEN_KEY);
  const rawUser = localStorage.getItem(AUTH_USER_KEY);

  try {
    return {
      token: rawToken,
      user: rawUser ? (JSON.parse(rawUser) as StoredUser) : null,
    };
  } catch {
    return { token: rawToken, user: null };
  }
}

export function persistSession(token: string, user: StoredUser): void {
  if (!isBrowser()) {
    return;
  }
  ensureAdminCreator(user);
  localStorage.setItem(AUTH_TOKEN_KEY, token);
  localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
}

export function clearSession(): void {
  if (!isBrowser()) {
    return;
  }
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_USER_KEY);
  localStorage.removeItem(MOVIES_CACHE_KEY);
}

export function loadMoviesCache<T = unknown>(): MoviesCacheSnapshot<T> | null {
  if (!isBrowser()) {
    return null;
  }
  const raw = localStorage.getItem(MOVIES_CACHE_KEY);
  if (!raw) {
    return null;
  }
  try {
    return JSON.parse(raw) as MoviesCacheSnapshot<T>;
  } catch {
    return null;
  }
}

export function persistMoviesCache<T>(items: T[]): void {
  if (!isBrowser()) {
    return;
  }
  const snapshot: MoviesCacheSnapshot<T> = {
    items,
    timestamp: Date.now(),
  };
  localStorage.setItem(MOVIES_CACHE_KEY, JSON.stringify(snapshot));
}

export function clearMoviesCache(): void {
  if (!isBrowser()) {
    return;
  }
  localStorage.removeItem(MOVIES_CACHE_KEY);
}
