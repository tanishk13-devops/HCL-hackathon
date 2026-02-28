export interface User {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  passwordHash?: string;
  role: 'Admin' | 'Customer';
  createdAt: Date;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}
