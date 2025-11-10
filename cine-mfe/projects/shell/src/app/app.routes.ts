import { Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/module-federation';
import { authGuard, authGuardChild, authMatchGuard } from './core/auth.guard';
import { roleGuard, roleMatchGuard } from './core/role.guard';

const remoteEntries = {
  movies: 'http://localhost:4202/remoteEntry.js',
  users: 'http://localhost:4201/remoteEntry.js',
} as const;

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.component').then((m) => m.LoginComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    canActivateChild: [authGuardChild],
    canMatch: [authMatchGuard],
    loadComponent: () =>
      import('./layout/layout.component').then((m) => m.LayoutComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'movies' },
      {
        path: 'movies',
        loadChildren: () =>
          loadRemoteModule({
            type: 'module',
            remoteEntry: remoteEntries.movies,
            exposedModule: './Routes',
          }).then((m) => m.routes),
      },
      {
        path: 'users',
        canActivate: [roleGuard],
        canMatch: [roleMatchGuard],
        data: { roles: ['ADMIN'] },
        loadChildren: () =>
          loadRemoteModule({
            type: 'module',
            remoteEntry: remoteEntries.users,
            exposedModule: './Routes',
          }).then((m) => m.routes),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
