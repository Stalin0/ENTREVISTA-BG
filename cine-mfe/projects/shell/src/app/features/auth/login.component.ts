import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  computed,
  inject,
  signal,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService, RegisterRequest } from '../../core/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

type AuthMode = 'login' | 'register-admin';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  mode = signal<AuthMode>('login');
  loading = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  loginForm = this.fb.nonNullable.group({
    correo: ['', [Validators.required, Validators.email]],
    contrasenia: ['', Validators.required],
  });

  adminForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    correo: ['', [Validators.required, Validators.email]],
    contrasenia: ['', [Validators.required, Validators.minLength(6)]],
    usuarioCreacion: [''],
  });

  readonly isRegisterMode = computed(() => this.mode() === 'register-admin');

  toggleMode(next: AuthMode): void {
    this.mode.set(next);
    this.successMessage.set(null);
    this.errorMessage.set(null);
    if (next === 'register-admin') {
      const correo = this.adminForm.controls.correo.value;
      this.adminForm.controls.usuarioCreacion.setValue(
        correo || 'usuario.admin',
      );
    }
  }

  async submitLogin(): Promise<void> {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    const credentials = this.loginForm.getRawValue();

    try {
      await firstValueFrom(this.auth.login(credentials));
      const returnUrl =
        this.route.snapshot.queryParamMap.get('returnUrl') ?? '/movies';
      await this.router.navigateByUrl(returnUrl);
    } catch (error) {
      this.errorMessage.set(this.resolveErrorMessage(error));
    } finally {
      this.loading.set(false);
    }
  }

  submitAdmin(): void {
    if (this.adminForm.invalid) {
      this.adminForm.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const payload = this.adminForm.getRawValue() as RegisterRequest;
    console.log("LOGIN", payload)
    if (!payload.usuario) {
      payload.usuario = payload.correo;
    }

    this.auth
      .registerAdmin(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.successMessage.set(
            'Administrador registrado correctamente. Ahora puedes iniciar sesión.',
          );
          this.mode.set('login');
          this.loginForm.controls.correo.setValue(payload.correo);
          this.loginForm.controls.contrasenia.setValue(payload.contrasenia);
        },
        error: (error) => {
          this.errorMessage.set(this.resolveErrorMessage(error));
        },
        complete: () => this.loading.set(false),
      });
  }

  private resolveErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error === 'string' && error.error.length > 0) {
        return error.error;
      }
      if (error.error?.message) {
        return error.error.message;
      }
      return `Error ${error.status}: ${error.statusText || 'Solicitud inválida'}`;
    }

    if (error instanceof Error) {
      return error.message;
    }

    return 'Ocurrió un error inesperado. Intenta nuevamente.';
  }
}
