import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../services/auth';
import { UserStateService } from '../../../services/user-state.service';
import { PlatformSettingsService } from '../../../services/platform-settings.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, CommonModule, MatIconModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class SidebarComponent implements OnInit {
  userName: string = 'Admin User';
  userEmail: string = 'admin@company.com';
  userImage: string | null = null;
  showDropdown: boolean = false;
  platformName: string = 'LearnTrack';
  sidebarOpen: boolean = false;

  constructor(
    private authService: AuthService, 
    private router: Router,
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
      this.userName = userData.fullName || userData.name || 'Admin User';
      this.userEmail = userData.email || 'admin@company.com';
      this.userImage = userData.profileImageBase64 ? `data:image/jpeg;base64,${userData.profileImageBase64}` : null;
      console.log('Sidebar - User image loaded:', !!this.userImage);
    }
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  logout(): void {
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
