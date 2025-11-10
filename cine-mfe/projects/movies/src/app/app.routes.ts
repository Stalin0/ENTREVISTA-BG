import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/movies-dashboard/movies-dashboard.component').then(
        (m) => m.MoviesDashboardComponent,
      ),
  },
];
