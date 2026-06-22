import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.currentUser?.accessToken;

  // clonamos a requisição original injetando o access token atual
  const authReq = token
    ? req.clone({ headers: req.headers.set('Authorization', `Bearer ${token}`) })
    : req;

  return next(authReq).pipe(
    // catchError intercepta qualquer erro que a requisição possa gerar
    catchError((error: HttpErrorResponse) => {
      // só tentamos renovar o token se o erro for 401 E o usuário tiver um refresh token
      // requisições de login/register que falham com 401 não devem entrar nesse fluxo
      const isAuthEndpoint = req.url.includes('/auth/login') || req.url.includes('/auth/register');

      if (error.status === 401 && authService.currentUser && !isAuthEndpoint) {
        // chamamos o refresh e, se der certo, repetimos a requisição original
        // com o novo token — usando switchMap para encadear os dois observables
        return authService.refreshToken().pipe(
          switchMap(newAuth => {
            const retryReq = req.clone({
              headers: req.headers.set('Authorization', `Bearer ${newAuth.accessToken}`)
            });
            return next(retryReq);
          }),
          catchError(refreshError => {
            // se o refresh também falhar (refresh token expirado), aí sim deslogamos
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }

      // qualquer outro erro (404, 500, etc.) simplesmente propaga normalmente
      return throwError(() => error);
    })
  );
};