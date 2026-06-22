import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  // no SSR não temos localStorage, então deixamos passar
  // o cliente vai redirecionar se necessário
  if (!isPlatformBrowser(platformId)) return true;

  if (authService.isAuthenticated) {
    return true;
  }

  router.navigate(['/auth/login']);
  return false;
};