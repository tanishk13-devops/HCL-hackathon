export interface User {
  id?: number;
  email: string;
  name: string;
  phone?: string;
  role: 'Customer' | 'Admin';
  token?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  phone: string;
}
