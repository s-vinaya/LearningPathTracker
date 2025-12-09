import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-manager-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ManagerProfileComponent implements OnInit {
  profile: any = null;
  isEditing = false;
  loading = false;
  message = '';
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  private apiUrl = 'https://localhost:7028/api/manager';
  managerId: number;
  departments: any[] = [];

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.managerId = this.authService.getCurrentUserId();
  }

  ngOnInit(): void {
    this.loadProfile();
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.authService.getDepartments().subscribe({
      next: (departments: any[]) => {
        this.departments = departments;
      },
      error: (err) => {
        console.error('Failed to load departments', err);
      }
    });
  }

  loadProfile(): void {
    this.loading = true;
    this.http.get<any>(`${this.apiUrl}/${this.managerId}/profile`).subscribe({
      next: (data) => {
        this.profile = {
          fullName: data.name || data.fullName,
          email: data.email,
          phoneNumber: data.phoneNumber,
          department: data.department,
          jobTitle: data.jobTitle
        };
        if (data.profileImageBase64) {
          this.previewUrl = `data:image/jpeg;base64,${data.profileImageBase64}`;
        }
        this.loading = false;
      },
      error: (err) => {
        this.message = 'Error loading profile';
        this.loading = false;
      }
    });
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    this.message = '';
  }

  saveProfile(): void {
    if (!this.profile) return;

    this.loading = true;
    this.http.put('https://localhost:7028/api/profile', {
      fullName: this.profile.fullName,
      phoneNumber: this.profile.phoneNumber,
      department: this.profile.department,
      jobTitle: this.profile.jobTitle
    }).subscribe({
      next: () => {
        this.message = 'Profile updated successfully';
        this.isEditing = false;
        this.loading = false;
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        user.name = this.profile.fullName;
        user.fullName = this.profile.fullName;
        user.department = this.profile.department;
        localStorage.setItem('user', JSON.stringify(user));
      },
      error: (err) => {
        this.message = err.error?.message || 'Error updating profile';
        this.loading = false;
      }
    });
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
    
    const formData = new FormData();
    formData.append('file', this.selectedFile);
    
    this.loading = true;
    this.http.post(`${this.apiUrl}/${this.managerId}/profile/image`, formData).subscribe({
      next: () => {
        this.message = 'Profile image updated successfully';
        this.selectedFile = null;
        this.loadProfile();
      },
      error: () => {
        this.message = 'Error uploading image';
        this.loading = false;
      }
    });
  }

  cancel(): void {
    this.isEditing = false;
    this.loadProfile();
  }
}
