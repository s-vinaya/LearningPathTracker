import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../services/auth';
import { UserStateService } from '../../../services/user-state.service';
import { PlatformSettingsService } from '../../../services/platform-settings.service';

@Component({
  selector: 'app-employee-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, MatIconModule],
  templateUrl: './employee-sidebar.html',
  styleUrls: ['./employee-sidebar.css']
})
export class EmployeeSidebarComponent implements OnInit {
  userName: string = 'Employee';
  userEmail: string = 'employee@company.com';
  userImage: string | null = null;
  showDropdown: boolean = false;
  platformName: string = 'LearnTrack';
  sidebarOpen: boolean = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    private userStateService: UserStateService,
    private settingsService: PlatformSettingsService
  ) {}

  ngOnInit(): void {
    this.loadUserData();
    this.userStateService.userImage$.subscribe(image => {
      if (image) {
        this.userImage = `data:image/jpeg;base64,${image}`;
      }
    });
    this.settingsService.settings$.subscribe(settings => {
      this.platformName = settings.platformName;
    });
  }

  loadUserData(): void {
    const user = localStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      this.userName = userData.fullName || userData.name || 'Employee';
      this.userEmail = userData.email || 'employee@company.com';
      this.userImage = userData.profileImageBase64 ? `data:image/jpeg;base64,${userData.profileImageBase64}` : null;
    }
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
    this.showDropdown = false;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
