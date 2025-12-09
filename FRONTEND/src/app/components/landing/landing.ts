import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-landing',
  imports: [CommonModule, MatButtonModule, MatIconModule, FormsModule],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  features = [
    { icon: 'school', title: 'Expert-Led Courses', desc: 'Learn from industry professionals' },
    { icon: 'emoji_events', title: 'Earn Certificates', desc: 'Validate your achievements' },
    { icon: 'trending_up', title: 'Track Progress', desc: 'Monitor your learning journey' },
    { icon: 'groups', title: 'Team Learning', desc: 'Collaborate with colleagues' }
  ];

  stats = [
    { value: '10K+', label: 'Active Learners' },
    { value: '500+', label: 'Courses' },
    { value: '95%', label: 'Success Rate' },
    { value: '24/7', label: 'Support' }
  ];

  certificateId = '';
  certificateData: any = null;
  verifyError = '';
  verifying = false;

  constructor(private router: Router, private http: HttpClient) {}

  navigateToLogin() {
    this.router.navigate(['/login']);
  }

  navigateToRegister() {
    this.router.navigate(['/register']);
  }

  scrollToVerify() {
    document.getElementById('verify')?.scrollIntoView({ behavior: 'smooth' });
  }

  verifyCertificate() {
    if (!this.certificateId.trim()) {
      this.verifyError = 'Please enter a certificate ID';
      return;
    }

    this.verifying = true;
    this.verifyError = '';
    this.certificateData = null;

    this.http.get(`https://localhost:7028/api/certificates/verify/${this.certificateId}`)
      .subscribe({
        next: (data: any) => {
          this.certificateData = data;
          this.verifying = false;
        },
        error: (err) => {
          this.verifyError = 'Certificate not found or invalid';
          this.verifying = false;
        }
      });
  }
}
