import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // URL base da API — em produção viria do environment
  private readonly apiUrl = 'http://localhost:5125/api';

  // BehaviorSubject mantém o estado do usuário logado
  // qualquer componente pode se inscrever e saber se o usuário está autenticado
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(
    this.getUserFromStorage()
  );

  // observable público — os componentes usam esse, não o subject diretamente
  // isso é encapsulamento — ninguém de fora consegue emitir valores
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  get currentUser(): AuthResponse | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return !!this.currentUser;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request).pipe(
      // tap executa um efeito colateral sem modificar o valor do observable
      // usamos para salvar o token e atualizar o estado
      tap(response => this.setSession(response))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, request).pipe(
      tap(response => this.setSession(response))
    );
  }

    logout(): void {
    if (typeof window !== 'undefined') {
        localStorage.removeItem('auth_user');
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
    }

  // salva o usuário no localStorage e atualiza o estado
    private setSession(response: AuthResponse): void {
    if (typeof window !== 'undefined') {
        localStorage.setItem('auth_user', JSON.stringify(response));
    }
    this.currentUserSubject.next(response);
    }

  // recupera o usuário do localStorage ao iniciar a aplicação
    private getUserFromStorage(): AuthResponse | null {
    // verificamos se estamos no navegador antes de acessar o localStorage
    // no SSR o código roda no servidor onde localStorage não existe
    if (typeof window === 'undefined') return null;
    const user = localStorage.getItem('auth_user');
    return user ? JSON.parse(user) : null;
    }

    // chama o endpoint de refresh para conseguir um novo par de tokens
    refreshToken(): Observable<AuthResponse> {
      const currentRefreshToken = this.currentUser?.refreshToken;

      return this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, {
        refreshToken: currentRefreshToken
      }).pipe(
        tap(response => this.setSession(response))
      );
    }
}