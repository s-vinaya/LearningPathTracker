import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../services/auth';
import { PlatformSettingsService } from '../../../services/platform-settings.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-manager-sidebar',
  imports: [CommonModule, RouterModule, MatIconModule],
  templateUrl: './manager-sidebar.component.html',
  styleUrl: './manager-sidebar.css'
})
export class ManagerSidebarComponent implements OnInit {
  @Input() managerProfile: any = {};
  showDropdown = false;
  userName = 'Manager Name';
  userEmail = 'manager@company.com';
  profileImage: string | null = null;
  platformName: string = 'LearnTrack';
  sidebarOpen: boolean = false;
  private apiUrl = 'https://localhost:7028/api/manager';

  constructor(
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    private settingsService: PlatformSettingsService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
  }

  loadProfile(): void {
    const managerId = this.authService.getCurrentUserId();
    this.http.get<any>(`${this.apiUrl}/${managerId}/profile`).subscribe({
      next: (data) => {
        this.userName = data.name || data.fullName || 'Manager Name';
        this.userEmail = data.email || 'manager@company.com';
        if (data.profileImageBase64) {
          this.profileImage = `data:image/jpeg;base64,${data.profileImageBase64}`;
        }
      },
      error: () => console.error('Failed to load sidebar profile')
    });
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
  }
}
