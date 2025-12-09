import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../services/auth';


@Component({
  selector: 'app-manager-settings',
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="dashboard-container">
      <div class="header">
        <div class="welcome">
          <h1>Settings</h1>
          <p>Manage your profile, notifications, and team settings.</p>
        </div>
      </div>
      <div class="content">


          <!-- Profile Settings -->
          <div class="settings-section">
            <div class="section-header">
              <i class="fas fa-user"></i>
              <h3>Profile Settings</h3>
            </div>
            <div class="settings-form">
              <div class="form-row">
                <div class="form-group">
                  <label>Full Name</label>
                  <input type="text" class="form-input" [(ngModel)]="profileSettings.fullName">
                </div>
                <div class="form-group">
                  <label>Email</label>
                  <input type="email" class="form-input" [(ngModel)]="profileSettings.email">
                </div>
              </div>
              <div class="form-row">
                <div class="form-group">
                  <label>Department</label>
                  <select class="form-select" [(ngModel)]="profileSettings.department">
                    <option value="Engineering">Engineering</option>
                    <option value="Design">Design</option>
                    <option value="Marketing">Marketing</option>
                  </select>
                </div>
                <div class="form-group">
                  <label>Team</label>
                  <input type="text" class="form-input" [(ngModel)]="profileSettings.team">
                </div>
              </div>
              <button class="update-btn" (click)="updateProfile()" [disabled]="loading">Update Profile</button>
            </div>
          </div>

          <!-- Notification Settings -->
          <div class="settings-section">
            <div class="section-header">
              <i class="fas fa-bell"></i>
              <h3>Notification Settings</h3>
            </div>
            <div class="notification-settings">
              <div class="notification-row">
                <div class="notification-group">
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.emailNotifications">
                    <span>Email Notifications</span>
                  </label>
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.weeklyProgressReports">
                    <span>Weekly Progress Reports</span>
                  </label>
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.courseCompletionAlerts">
                    <span>Course Completion Alerts</span>
                  </label>
                </div>
                <div class="notification-group">
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.assignmentReminders">
                    <span>Assignment Reminders</span>
                  </label>
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.teamUpdates">
                    <span>Team Updates</span>
                  </label>
                  <label class="checkbox-label">
                    <input type="checkbox" [(ngModel)]="notificationSettings.systemNotifications">
                    <span>System Notifications</span>
                  </label>
                </div>
              </div>
              <button class="save-btn" (click)="saveNotifications()" [disabled]="loading">Save Preferences</button>
            </div>
          </div>

          <!-- Team Settings -->
          <div class="settings-section">
            <div class="section-header">
              <i class="fas fa-users-cog"></i>
              <h3>Team Settings</h3>
            </div>
            <div class="team-settings">
              <div class="form-row">
                <div class="form-group">
                  <label>Default Learning Hours/Week</label>
                  <input type="number" class="form-input" [(ngModel)]="teamSettings.defaultLearningHours">
                </div>
                <div class="form-group">
                  <label>Assignment Deadline (Days)</label>
                  <input type="number" class="form-input" [(ngModel)]="teamSettings.assignmentDeadline">
                </div>
              </div>
              <div class="form-row">
                <div class="form-group">
                  <label>Reminder Frequency</label>
                  <select class="form-select" [(ngModel)]="teamSettings.reminderFrequency">
                    <option value="Weekly">Weekly</option>
                    <option value="Bi-weekly">Bi-weekly</option>
                    <option value="Monthly">Monthly</option>
                  </select>
                </div>
                <div class="form-group">
                  <label>Auto-assign New Paths</label>
                  <select class="form-select" [(ngModel)]="teamSettings.autoAssignNewPaths">
                    <option value="Disabled">Disabled</option>
                    <option value="Enabled">Enabled</option>
                  </select>
                </div>
              </div>
              <button class="update-btn" (click)="updateTeamSettings()" [disabled]="loading">Update Settings</button>
            </div>
          </div>
      </div>
    </div>
  `,
  styleUrls: ['../dashboard/manager-dashboard.css', './settings.css']
})
export class ManagerSettingsComponent implements OnInit {
  private apiUrl = 'https://localhost:7028/api/manager';
  managerId: number;
  loading = false;
  profileSettings = {
    fullName: 'Manager Name',
    email: 'manager@company.com',
    department: 'Engineering',
    team: 'Development Team'
  };

  notificationSettings = {
    emailNotifications: true,
    weeklyProgressReports: true,
    courseCompletionAlerts: false,
    assignmentReminders: true,
    teamUpdates: false,
    systemNotifications: false
  };

  teamSettings = {
    defaultLearningHours: 8,
    assignmentDeadline: 14,
    reminderFrequency: 'Weekly',
    autoAssignNewPaths: 'Disabled'
  };

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {
    this.managerId = this.authService.getCurrentUserId();
  }

  ngOnInit(): void {
    this.loadProfile();
    this.loadNotifications();
    this.loadTeamSettings();
  }

  loadProfile(): void {
    this.loading = true;
    this.http.get<any>(`${this.apiUrl}/${this.managerId}/profile`).subscribe({
      next: (data) => {
        this.profileSettings.fullName = data.name || data.fullName;
        this.profileSettings.email = data.email;
        this.profileSettings.department = data.department;
        this.profileSettings.team = data.jobTitle || 'Development Team';
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        alert('Failed to load profile');
      }
    });
  }

  loadNotifications(): void {
    this.http.get<any>(`${this.apiUrl}/${this.managerId}/notifications`).subscribe({
      next: (data) => {
        this.notificationSettings = data;
      },
      error: () => console.error('Failed to load notifications')
    });
  }

  loadTeamSettings(): void {
    this.http.get<any>(`${this.apiUrl}/${this.managerId}/team-settings`).subscribe({
      next: (data) => {
        this.teamSettings = data;
      },
      error: () => console.error('Failed to load team settings')
    });
  }

  updateProfile(): void {
    this.loading = true;
    this.http.put(`${this.apiUrl}/${this.managerId}/profile`, this.profileSettings).subscribe({
      next: () => {
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        user.name = this.profileSettings.fullName;
        user.fullName = this.profileSettings.fullName;
        user.email = this.profileSettings.email;
        user.department = this.profileSettings.department;
        localStorage.setItem('user', JSON.stringify(user));
        this.loading = false;
        alert('Profile updated successfully');
        this.router.navigate(['/manager/dashboard']).then(() => window.location.reload());
      },
      error: () => {
        this.loading = false;
        alert('Failed to update profile');
      }
    });
  }

  saveNotifications(): void {
    this.loading = true;
    this.http.put(`${this.apiUrl}/${this.managerId}/notifications`, this.notificationSettings).subscribe({
      next: () => {
        this.loading = false;
        alert('Notification preferences saved');
      },
      error: () => {
        this.loading = false;
        alert('Failed to save preferences');
      }
    });
  }

  updateTeamSettings(): void {
    this.loading = true;
    this.http.put(`${this.apiUrl}/${this.managerId}/team-settings`, this.teamSettings).subscribe({
      next: () => {
        this.loading = false;
        alert('Team settings updated');
      },
      error: () => {
        this.loading = false;
        alert('Failed to update settings');
      }
    });
  }
}