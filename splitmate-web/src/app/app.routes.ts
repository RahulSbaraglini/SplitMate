import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },

  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register').then(m => m.RegisterComponent)
      }
    ]
  },

  {
    path: 'groups',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/groups/groups/groups').then(m => m.GroupsComponent)
  },

  {
    path: 'groups/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/groups/group-detail/group-detail').then(m => m.GroupDetailComponent)
  },

  { path: '**', redirectTo: 'auth/login' }
];