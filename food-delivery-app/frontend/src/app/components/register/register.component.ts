import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  phone = '';
  loading = false;
  error = '';
  success = '';

  constructor(private authService: AuthService, private router: Router) {}

  register(): void {
    const name = this.name.trim();
    const email = this.email.trim().toLowerCase();
    const password = this.password.trim();
    const phone = this.phone.trim();

    if (!name || !email || !password || !phone) {
      this.error = 'Please fill in all fields';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';
    this.authService.register({ name, email, password, phone, role: 'Customer' })
      .subscribe({
        next: () => {
          this.loading = false;
          this.error = '';
          this.success = 'Registration successful! Redirecting...';
          this.name = '';
          this.email = '';
          this.password = '';
          this.phone = '';
          setTimeout(() => {
            this.router.navigate(['/restaurants']);
          }, 2000);
        },
        error: (err) => {
          this.loading = false;
          this.error = this.extractError(err, 'Registration failed. Please try a different email.');
        }
      });
  }

  private extractError(err: any, fallback: string): string {
    if (typeof err?.error === 'string' && err.error.trim()) {
      return err.error;
    }

    if (typeof err?.error?.message === 'string' && err.error.message.trim()) {
      return err.error.message;
    }

    if (typeof err?.message === 'string' && err.message.trim()) {
      return err.message;
    }

    return fallback;
  }
}
