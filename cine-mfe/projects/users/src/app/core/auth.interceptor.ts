import { HttpInterceptorFn } from '@angular/common/http';
import { loadSession } from '@shared/auth/auth-storage';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const { token } = loadSession();
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
  return next(req);
};
