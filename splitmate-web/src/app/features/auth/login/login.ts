import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  // FormGroup representa o formulário inteiro
  // cada campo tem suas próprias validações — Clean Code aplicado ao frontend
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,    // FormBuilder facilita a criação de formulários
    private authService: AuthService,
    private router: Router
  ) {
    // definimos os campos e suas validações
    // Validators.required = campo obrigatório
    // Validators.email = precisa ser um email válido
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    // não faz nada se o formulário tiver erros de validação
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        // em caso de sucesso navega para a lista de grupos
        this.router.navigate(['/groups']);
      },
      error: (err) => {
        // em caso de erro exibe a mensagem para o usuário
        this.errorMessage = err.error?.message || 'E-mail ou senha inválidos.';
        this.isLoading = false;
      }
    });
  }
}