import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  UsersApiService,
  UserDto,
  ApiMessageResponse,
} from '../../data-access/users-api.service';
import { HttpErrorResponse } from '@angular/common/http';
import { loadSession } from '@shared/auth/auth-storage';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.scss',
})
export class UserDashboardComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly usersApi = inject(UsersApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly themeKey = 'cine.bg.theme';

  readonly currentAdmin = computed(() => loadSession().user);
  readonly themeMode = signal<'dark' | 'light'>('dark');

  readonly inviteForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    correo: ['', [Validators.required, Validators.email]],
    contrasenia: ['', [Validators.required, Validators.minLength(6)]],
  });

  readonly editForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    correo: ['', [Validators.required, Validators.email]],
    activo: [true],
  });

  readonly loadingList = signal(false);
  readonly saving = signal(false);
  readonly feedback = signal<string | null>(null);
  readonly error = signal<string | null>(null);
  readonly invitedUsers = signal<UserDto[]>([]);
  readonly editingUser = signal<UserDto | null>(null);

  ngOnInit(): void {
    this.initTheme();
    this.loadUsers();
  }

  loadUsers(): void {
    this.loadingList.set(true);
    this.error.set(null);
    this.usersApi
      .listUsers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (users) => {
          const enriched = (users ?? []).map((user) => this.normalizeUser(user));
          const invited = enriched.filter((user) => this.isInvitedUser(user));
          this.invitedUsers.set(invited);
        },
        error: (err) => {
          this.error.set(this.resolveError(err));
        },
        complete: () => this.loadingList.set(false),
      });
  }

  registerInvite(): void {
    if (this.inviteForm.invalid) {
      this.inviteForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    this.feedback.set(null);

    const payload = this.inviteForm.getRawValue();
    this.usersApi
      .createInvite(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.feedback.set('Invitado registrado correctamente.');
          this.inviteForm.reset();
          this.loadUsers();
        },
        error: (err) => {
          this.error.set(this.resolveError(err));
        },
        complete: () => this.saving.set(false),
      });
  }

  startEdit(user: UserDto): void {
    const normalizedUser = this.normalizeUser(user);
    this.editingUser.set(normalizedUser);
    this.editForm.reset({
      fullName: normalizedUser.fullName ?? '',
      correo: normalizedUser.correo ?? '',
      activo: this.isActiveUser(normalizedUser),
    });
  }

  cancelEdit(): void {
    this.editForm.reset({
      fullName: '',
      correo: '',
      activo: true,
    });
    this.editingUser.set(null);
  }

  toggleTheme(): void {
    const next = this.themeMode() === 'dark' ? 'light' : 'dark';
    this.themeMode.set(next);
    this.applyTheme(next);
    this.persistTheme(next);
  }

  submitEdit(): void {
    const current = this.editingUser();
    const userId = this.resolveUserId(current);
    if (!userId) {
      this.error.set('No se pudo determinar el identificador del invitado.');
      return;
    }
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);

    const payload = this.editForm.getRawValue();
    this.usersApi
      .updateUser(userId, payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: ApiMessageResponse) => {
          this.feedback.set(response?.message ?? 'Invitado actualizado.');
          this.invitedUsers.update((current) =>
            (current ?? []).map((user) =>
              this.resolveUserId(user) === userId ? { ...user, activo: payload.activo } : user,
            ),
          );
          this.cancelEdit();
          this.loadUsers();
        },
        error: (err) => {
          this.error.set(this.resolveError(err));
        },
        complete: () => this.saving.set(false),
      });
  }

  removeInvite(user: UserDto): void {
    const userId = this.resolveUserId(user);
    if (!userId) {
      this.error.set('No se pudo determinar el identificador del invitado.');
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    this.usersApi
      .deleteUser(userId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: ApiMessageResponse) => {
          this.feedback.set(
            response?.message ?? `Se elimino a ${user.fullName ?? user.correo}.`,
          );
          if (this.resolveUserId(this.editingUser()) === userId) {
            this.cancelEdit();
          }
          this.loadUsers();
        },
        error: (err) => {
          this.error.set(this.resolveError(err));
        },
        complete: () => this.saving.set(false),
      });
  }

  getInitial(user: UserDto): string {
    const seed =
      user.fullName?.trim() ||
      user.correo?.trim() ||
      (user as { usuario?: string }).usuario ||
      '?';
    return seed.charAt(0).toUpperCase();
  }

  statusLabel(user: UserDto): string {
    return this.isActiveUser(user) ? 'Activo' : 'Inactivo';
  }

  isActiveUser(user: UserDto | null | undefined): boolean {
    if (!user) {
      return false;
    }
    const raw = user['activo'];
    if (typeof raw === 'boolean') {
      return raw;
    }
    if (typeof raw === 'string') {
      const value = raw.trim().toUpperCase();
      if (
        value.includes('INACT') ||
        value.includes('NO ACT') ||
        value.includes('NO_ACT') ||
        value.includes('DESACT') ||
        value === 'NO' ||
        value === 'FALSE'
      ) {
        return false;
      }
      if (
        value.includes('ACT') ||
        value === 'SI' ||
        value === 'TRUE'
      ) {
        return true;
      }
    }
    return true;
  }

  private attachResolvedId(user: UserDto | null | undefined): UserDto {
    if (!user) {
      return user as unknown as UserDto;
    }
    const resolvedId = this.resolveUserId(user);
    if (resolvedId && user.id !== resolvedId) {
      return { ...user, id: resolvedId };
    }
    return user;
  }

  private normalizeUser(user: UserDto | null | undefined): UserDto {
    const withId = this.attachResolvedId(user);
    if (!withId) {
      return withId as unknown as UserDto;
    }
    const booleanActive = this.isActiveUser(withId);
    if (
      typeof withId['activo'] === 'boolean' &&
      withId['activo'] === booleanActive
    ) {
      return withId;
    }
    return { ...withId, activo: booleanActive };
  }

  private resolveUserId(user: UserDto | null | undefined): number | null {
    if (!user) {
      return null;
    }
    const candidates = [
      user.id,
      (user as { idUsuario?: number | string }).idUsuario,
      (user as { idUsuarios?: number | string }).idUsuarios,
      (user as { id_user?: number | string }).id_user,
      (user as { userId?: number | string }).userId,
      (user as { usuarioId?: number | string }).usuarioId,
      (user as { usuario_id?: number | string }).usuario_id,
    ];
    for (const candidate of candidates) {
      if (typeof candidate === 'number' && Number.isFinite(candidate)) {
        return candidate;
      }
      if (typeof candidate === 'string' && candidate.trim().length) {
        const parsed = Number(candidate.trim());
        if (Number.isFinite(parsed)) {
          return parsed;
        }
      }
    }
    return null;
  }

  private isInvitedUser(user: UserDto | null | undefined): boolean {
    if (!user) {
      return false;
    }

    if (typeof user.idRol === 'number') {
      return user.idRol !== 1;
    }

    const role = this.resolveRoleLabel(user);
    if (role === 'ADMIN') {
      return false;
    }
    if (role === 'INVITED') {
      return true;
    }

    const estado = (user['activo'] ?? user['status']) as string | undefined;
    if (typeof estado === 'string' && estado.trim().length) {
      const upperEstado = estado.trim().toUpperCase();
      if (upperEstado.includes('INACT')) {
        return false;
      }
      if (upperEstado.includes('ACT')) {
        return true;
      }
    }

    return true;
  }

  private resolveRoleLabel(
    user: UserDto | null | undefined,
  ): 'ADMIN' | 'INVITED' | null {
    if (!user) {
      return null;
    }
    const candidates = [
      user.role,
      (user as { rol?: string }).rol,
      (user as { nombreRol?: string }).nombreRol,
    ];
    for (const candidate of candidates) {
      if (typeof candidate === 'string' && candidate.trim().length) {
        const upper = candidate.trim().toUpperCase();
        if (upper.includes('ADMIN')) {
          return 'ADMIN';
        }
        if (upper.includes('INV')) {
          return 'INVITED';
        }
      }
    }
    return null;
  }

  private initTheme(): void {
    const stored = this.readStoredTheme();
    this.themeMode.set(stored);
    this.applyTheme(stored);
  }

  private applyTheme(mode: 'dark' | 'light'): void {
    if (!this.isBrowserEnvironment()) {
      return;
    }
    document.body.dataset['cbgTheme'] = mode;
  }

  private persistTheme(mode: 'dark' | 'light'): void {
    if (!this.isBrowserEnvironment()) {
      return;
    }
    try {
      window.localStorage.setItem(this.themeKey, mode);
    } catch {
      // ignore storage errors
    }
  }

  private readStoredTheme(): 'dark' | 'light' {
    if (!this.isBrowserEnvironment()) {
      return 'dark';
    }
    const stored = window.localStorage.getItem(this.themeKey);
    return stored === 'light' ? 'light' : 'dark';
  }

  private isBrowserEnvironment(): boolean {
    return typeof window !== 'undefined' && typeof document !== 'undefined';
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
    return 'No fue posible completar la operacion.';
  }
}
