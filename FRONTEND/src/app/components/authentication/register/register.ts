import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { PlatformSettingsService } from '../../../services/platform-settings.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [FormsModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent implements OnInit {
  user = {
    fullName: '',
    email: '',
    password: '',
    phoneNumber: '',
    department: '',
    jobTitle: '',
    role: 'Employee'
  };
  confirmPassword = '';
  error = '';
  platformName = 'Learning Path Tracker';
  emailError = '';
  passwordError = '';
  confirmPasswordError = '';
  passwordStrength = { level: '', color: '', text: '' };
  departments: any[] = [];

  constructor(
    private authService: AuthService, 
    private router: Router,
    private settingsService: PlatformSettingsService
  ) {}

  ngOnInit(): void {
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
    this.loadDepartments();
  }

  loadDepartments() {
    this.authService.getDepartments().subscribe({
      next: (departments: any[]) => {
        this.departments = departments;
      },
      error: (err) => {
        console.error('Failed to load departments', err);
      }
    });
  }

  validateEmail() {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.user.email) {
      this.emailError = 'Email is required';
    } else if (!emailRegex.test(this.user.email)) {
      this.emailError = 'Invalid email format';
    } else {
      this.emailError = '';
    }
  }

  validatePassword() {
    const pwd = this.user.password;
    if (!pwd) {
      this.passwordError = 'Password is required';
      this.passwordStrength = { level: '', color: '', text: '' };
      return;
    }

    let strength = 0;
    if (pwd.length >= 8) strength++;
    if (pwd.length >= 12) strength++;
    if (/[a-z]/.test(pwd) && /[A-Z]/.test(pwd)) strength++;
    if (/\d/.test(pwd)) strength++;
    if (/[^a-zA-Z\d]/.test(pwd)) strength++;

    if (pwd.length < 6) {
      this.passwordError = 'Password must be at least 6 characters';
      this.passwordStrength = { level: 'weak', color: '#ef4444', text: 'Weak' };
    } else if (strength <= 2) {
      this.passwordError = '';
      this.passwordStrength = { level: 'weak', color: '#ef4444', text: 'Weak' };
    } else if (strength === 3) {
      this.passwordError = '';
      this.passwordStrength = { level: 'medium', color: '#f59e0b', text: 'Medium' };
    } else {
      this.passwordError = '';
      this.passwordStrength = { level: 'strong', color: '#10b981', text: 'Strong' };
    }

    this.validateConfirmPassword();
  }

  validateConfirmPassword() {
    if (!this.confirmPassword) {
      this.confirmPasswordError = 'Please confirm your password';
    } else if (this.user.password !== this.confirmPassword) {
      this.confirmPasswordError = 'Passwords do not match';
    } else {
      this.confirmPasswordError = '';
    }
  }

  onSubmit() {
    this.validateEmail();
    this.validatePassword();
    this.validateConfirmPassword();
    if (this.emailError || this.passwordError || this.confirmPasswordError) return;

    this.authService.register(this.user).subscribe({
      next: (res) => {
        this.showToast('Registration successful! Please login.');
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        if (err.status === 400 && err.error.message?.includes('already exists')) {
          this.showToast('Email already registered. Redirecting to login...');
          setTimeout(() => this.router.navigate(['/login']), 2000);
        } else {
          this.error = err.error?.message || 'Registration failed';
        }
      }
    });
  }

  showToast(message: string) {
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.classList.add('show'), 100);
    setTimeout(() => {
      toast.classList.remove('show');
      setTimeout(() => document.body.removeChild(toast), 300);
    }, 2000);
  }

  switchToLogin() {
    this.router.navigate(['/login']);
  }
}
