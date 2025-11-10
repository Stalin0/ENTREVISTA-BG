import { inject } from '@angular/core';
import {
  CanActivateChildFn,
  CanActivateFn,
  CanMatchFn,
  Router,
  UrlTree,
} from '@angular/router';
import { AuthService } from './auth.service';

function redirectToLogin(targetUrl?: string): UrlTree {
  const router = inject(Router);
  return router.createUrlTree(['/login'], {
    queryParams: targetUrl ? { returnUrl: targetUrl } : undefined,
  });
}

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  return auth.isLoggedIn() ? true : redirectToLogin(state.url);
};

export const authGuardChild: CanActivateChildFn = (_route, state) => {
  const auth = inject(AuthService);
  return auth.isLoggedIn() ? true : redirectToLogin(state.url);
};

export const authMatchGuard: CanMatchFn = (_route, segments) => {
  const auth = inject(AuthService);
  if (auth.isLoggedIn()) {
    return true;
  }
  const url = '/' + segments.map((segment) => segment.path).join('/');
  return redirectToLogin(url);
};
