import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { PlatformSettingsService } from '../../../services/platform-settings.service';

@Component({
  selector: 'app-reset-password',
  imports: [FormsModule, CommonModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css',
})
export class ResetPassword implements OnInit {
  token = '';
  newPassword = '';
  confirmPassword = '';
  message = '';
  error = '';
  loading = false;
  platformName = 'Learning Path Tracker';
  private apiUrl = 'https://localhost:7028/api/Auth';

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private settingsService: PlatformSettingsService
  ) {
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
  }

  ngOnInit() {
    this.token = this.route.snapshot.queryParams['token'] || '';
    if (!this.token) {
      this.error = 'Invalid reset link';
    }
  }

  onSubmit() {
    if (this.newPassword !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.loading = true;
    this.error = '';
    this.message = '';

    this.http.post(`${this.apiUrl}/reset-password`, { 
      token: this.token, 
      newPassword: this.newPassword 
    }).subscribe({
      next: () => {
        this.loading = false;
        this.message = 'Password reset successfully! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to reset password';
      }
    });
  }
}
