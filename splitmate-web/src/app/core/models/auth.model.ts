// representa o que mandamos para a API no login
export interface LoginRequest {
  email: string;
  password: string;
}

// representa o que mandamos para a API no cadastro
export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

// representa o que a API devolve após login ou cadastro
// espelhamos exatamente o AuthResponse do backend
export interface AuthResponse {
  userId: string;
  name: string;
  email: string;
  accessToken: string;
  refreshToken: string;
}