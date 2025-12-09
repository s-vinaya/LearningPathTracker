import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { PlatformSettingsService } from '../../../services/platform-settings.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit {
  email = '';
  password = '';
  error = '';
  platformName = 'Learning Path Tracker';
  emailError = '';
  passwordError = '';

  constructor(
    private authService: AuthService, 
    private router: Router,
    private settingsService: PlatformSettingsService
  ) {}

  ngOnInit(): void {
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
  }

  validateEmail() {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.email) {
      this.emailError = 'Email is required';
    } else if (!emailRegex.test(this.email)) {
      this.emailError = 'Invalid email format';
    } else {
      this.emailError = '';
    }
  }

  validatePassword() {
    if (!this.password) {
      this.passwordError = 'Password is required';
    } else if (this.password.length < 6) {
      this.passwordError = 'Password must be at least 6 characters';
    } else {
      this.passwordError = '';
    }
  }



  onSubmit() {
    this.validateEmail();
    this.validatePassword();
    if (this.emailError || this.passwordError) return;

    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        const role = res.user.role;
        if (role === 'Admin') {
          this.router.navigate(['/admin-dashboard']);
        } else if (role === 'Manager') {
          this.router.navigate(['/manager/dashboard']);
        } else {
          this.router.navigate(['/employee/dashboard']);
        }
      },
      error: (err) => {
        if (err.status === 401) {
          this.error = 'User not found. Redirecting to registration...';
          setTimeout(() => this.router.navigate(['/register']), 2000);
        } else {
          this.error = 'Invalid credentials';
        }
      }
    });
  }

  switchToRegister() {
    this.router.navigate(['/register']);
  }

  forgotPassword() {
    this.router.navigate(['/forgot-password']);
  }
}
