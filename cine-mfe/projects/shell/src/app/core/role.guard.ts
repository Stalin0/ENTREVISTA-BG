import { inject } from '@angular/core';
import { CanActivateFn, CanMatchFn, Router, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';

function isAllowed(roles: unknown, currentRole: string | null): boolean {
  if (!Array.isArray(roles) || roles.length === 0) {
    return true;
  }
  if (!currentRole) {
    return false;
  }
  return roles
    .map((role) => (typeof role === 'string' ? role.toUpperCase() : role))
    .includes(currentRole.toUpperCase());
}

function redirectToDefault(): UrlTree {
  return inject(Router).createUrlTree(['/movies']);
}

export const roleGuard: CanActivateFn = (route, _state) => {
  const auth = inject(AuthService);
  const required = route.data?.['roles'];
  return isAllowed(required, auth.role()) ? true : redirectToDefault();
};

export const roleMatchGuard: CanMatchFn = (route, _segments) => {
  const auth = inject(AuthService);
  const required = route.data?.['roles'];
  return isAllowed(required, auth.role()) ? true : redirectToDefault();
};
