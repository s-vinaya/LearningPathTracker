import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { SidebarComponent } from '../sidebar/sidebar';
import { ProfileService, UserProfile } from '../../../services/profile';
import { UserStateService } from '../../../services/user-state.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, MatIconModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  isEditing = false;
  loading = false;
  message = '';
  selectedFile: File | null = null;
  previewUrl: string | null = null;

  constructor(
    private profileService: ProfileService,
    private userStateService: UserStateService
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    console.log('Token exists:', !!token);
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('Token payload:', payload);
    }
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.profileService.getProfile().subscribe({
      next: (data) => {
        console.log('Profile data received:', data);
        this.profile = data;
        if (data.profileImageBase64) {
          this.previewUrl = `data:image/jpeg;base64,${data.profileImageBase64}`;
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading profile:', err);
        console.error('Error details:', err.error);
        console.error('Status:', err.status);
        this.message = `Error: ${err.status} - ${err.message}`;
        this.loading = false;
      }
    });
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    this.message = '';
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  uploadImage(): void {
    if (!this.selectedFile) return;
    
    this.loading = true;
    this.profileService.uploadProfileImage(this.selectedFile).subscribe({
      next: (res) => {
        this.message = 'Profile image updated successfully';
        this.selectedFile = null;
        this.loadProfile();
        this.updateLocalStorage();
      },
      error: (err) => {
        this.message = 'Error uploading image';
        this.loading = false;
      }
    });
  }

  updateLocalStorage(): void {
    this.profileService.getProfile().subscribe({
      next: (data) => {
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        user.profileImageBase64 = data.profileImageBase64;
        localStorage.setItem('user', JSON.stringify(user));
        this.userStateService.updateUserImage(data.profileImageBase64 || null);
      }
    });
  }

  saveProfile(): void {
    if (!this.profile) return;

    this.loading = true;
    this.profileService.updateProfile({
      fullName: this.profile.fullName,
      phoneNumber: this.profile.phoneNumber,
      department: this.profile.department,
      jobTitle: this.profile.jobTitle
    }).subscribe({
      next: (res) => {
        this.message = 'Profile updated successfully';
        this.isEditing = false;
        this.loading = false;
      },
      error: (err) => {
        this.message = 'Error updating profile';
        this.loading = false;
      }
    });
  }

  cancel(): void {
    this.isEditing = false;
    this.loadProfile();
  }
}
