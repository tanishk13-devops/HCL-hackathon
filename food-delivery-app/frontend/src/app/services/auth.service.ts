import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { User, LoginRequest, RegisterRequest } from '../models/user.model';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly userStorageKey = 'user';
  private readonly tokenStorageKey = 'token';
  private apiUrl = `${environment.apiUrl}/auth`;
  private userSubject = new BehaviorSubject<User | null>(this.getUserFromStorage());
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {
    window.addEventListener('storage', (event: StorageEvent) => {
      if (event.key === this.userStorageKey || event.key === this.tokenStorageKey) {
        this.userSubject.next(this.getUserFromStorage());
      }
    });
  }

  login(request: LoginRequest): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, request).pipe(tap(user => this.persistAuth(user)));
  }

  register(request: RegisterRequest): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, request).pipe(tap(user => this.persistAuth(user)));
  }

  logout(): void {
    localStorage.removeItem(this.userStorageKey);
    localStorage.removeItem(this.tokenStorageKey);
    this.userSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.userSubject.value && !!localStorage.getItem(this.tokenStorageKey);
  }

  getCurrentUser(): User | null {
    return this.userSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  private persistAuth(user: User): void {
    const token = (user.token || '').trim();
    localStorage.setItem(this.userStorageKey, JSON.stringify(user));
    localStorage.setItem(this.tokenStorageKey, token);
    this.userSubject.next({ ...user, token });
  }

  private getUserFromStorage(): User | null {
    const user = localStorage.getItem(this.userStorageKey);
    const token = localStorage.getItem(this.tokenStorageKey);

    if (!user || !token) {
      return null;
    }

    try {
      const parsed = JSON.parse(user) as User;
      return { ...parsed, token };
    } catch {
      return null;
    }
  }
}
