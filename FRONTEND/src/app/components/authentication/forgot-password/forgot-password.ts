import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { PlatformSettingsService } from '../../../services/platform-settings.service';

@Component({
  selector: 'app-forgot-password',
  imports: [FormsModule, CommonModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  email = '';
  message = '';
  error = '';
  loading = false;
  platformName = 'Learning Path Tracker';
  private apiUrl = 'https://localhost:7028/api/Auth';

  constructor(
    private http: HttpClient,
    private router: Router,
    private settingsService: PlatformSettingsService
  ) {
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
  }

  onSubmit() {
    this.loading = true;
    this.error = '';
    this.message = '';

    this.http.post(`${this.apiUrl}/forgot-password`, { email: this.email }).subscribe({
      next: () => {
        this.loading = false;
        this.message = 'Password reset link sent to your email!';
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Email not found';
      }
    });
  }

  backToLogin() {
    this.router.navigate(['/login']);
  }
}
