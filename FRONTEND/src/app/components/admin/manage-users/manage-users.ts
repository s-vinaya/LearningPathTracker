import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { SidebarComponent } from '../sidebar/sidebar';
import { UserService, User } from '../../../services/user.service';
import { HttpClient } from '@angular/common/http';
import jsPDF from 'jspdf';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

interface UserEnrollment {
  courseId: number;
  courseTitle: string;
  progress: number;
  enrolledAt: string;
  completedAt?: string;
}

interface UserDetails extends User {
  enrollments: UserEnrollment[];
}

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, MatIconModule],
  templateUrl: './manage-users.html',
  styleUrl: './manage-users.css'
})
export class ManageUsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  searchTerm: string = '';
  selectedStatus: string = 'all';
  selectedRole: string = 'all';
  loading: boolean = true;
  showViewModal: boolean = false;
  showEditModal: boolean = false;
  showExportModal: boolean = false;
  selectedUser: UserDetails | null = null;
  editForm: any = {};

  constructor(private userService: UserService, private http: HttpClient, private toast: ToastService, private confirm: ConfirmService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.filteredUsers = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.loading = false;
      }
    });
  }

  get totalUsers(): number {
    return this.users.length;
  }

  get activeUsers(): number {
    return this.users.filter(u => u.isActive).length;
  }

  get inactiveUsers(): number {
    return this.users.filter(u => !u.isActive).length;
  }

  get pendingApproval(): number {
    return this.users.filter(u => !u.isApproved).length;
  }

  filterUsers(): void {
    this.filteredUsers = this.users.filter(user => {
      const matchesSearch = user.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           user.email.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = this.selectedStatus === 'all' || 
                           (this.selectedStatus === 'active' && user.isActive) ||
                           (this.selectedStatus === 'inactive' && !user.isActive);
      const matchesRole = this.selectedRole === 'all' || user.role.toLowerCase() === this.selectedRole.toLowerCase();
      
      return matchesSearch && matchesStatus && matchesRole;
    });
  }

  deleteUser(id: number): void {
    this.confirm.show('Are you sure you want to delete this user?', () => {
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.toast.show('User deleted successfully', 'success');
          this.loadUsers();
        },
        error: (err) => {
          this.toast.show('Failed to delete user', 'error');
          console.error('Error deleting user:', err);
        }
      });
    });
  }

  approveUser(id: number): void {
    this.userService.approveUser(id).subscribe({
      next: () => this.loadUsers(),
      error: (err) => console.error('Error approving user:', err)
    });
  }

  changeRole(id: number, currentRole: string): void {
    const roles = ['Employee', 'Manager', 'Admin'];
    const newRole = prompt(`Change role from ${currentRole} to:`, currentRole);
    if (newRole && roles.includes(newRole)) {
      this.userService.updateUserRole(id, newRole).subscribe({
        next: () => this.loadUsers(),
        error: (err) => console.error('Error updating role:', err)
      });
    }
  }

  getTimeAgo(date: string): string {
    const now = new Date();
    const past = new Date(date);
    const diffMs = now.getTime() - past.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 60) return `${diffMins} minutes ago`;
    if (diffHours < 24) return `${diffHours} hours ago`;
    return `${diffDays} days ago`;
  }

  viewUser(userId: number): void {
    this.http.get<UserDetails>(`https://localhost:7028/api/admin/users/${userId}`).subscribe({
      next: (user) => {
        this.selectedUser = user;
        this.showViewModal = true;
      },
      error: (err) => console.error('Error loading user details:', err)
    });
  }

  editUser(userId: number): void {
    this.http.get<User>(`https://localhost:7028/api/admin/users/${userId}`).subscribe({
      next: (user) => {
        this.editForm = { ...user };
        this.showEditModal = true;
      },
      error: (err) => console.error('Error loading user:', err)
    });
  }

  closeModal(): void {
    this.showViewModal = false;
    this.showEditModal = false;
    this.selectedUser = null;
    this.editForm = {};
  }

  saveUser(): void {
    this.http.put(`https://localhost:7028/api/admin/users/${this.editForm.id}`, this.editForm).subscribe({
      next: () => {
        this.closeModal();
        this.loadUsers();
      },
      error: (err) => console.error('Error updating user:', err)
    });
  }

  openExportModal(): void {
    this.showExportModal = true;
  }

  closeExportModal(): void {
    this.showExportModal = false;
  }

  exportData(format: string): void {
    const data = this.filteredUsers;
    
    switch(format) {
      case 'pdf':
        this.exportToPDF(data);
        break;
      case 'excel':
        this.exportToExcel(data);
        break;
      case 'json':
        this.exportToJSON(data);
        break;
      case 'txt':
        this.exportToTXT(data);
        break;
    }
    this.closeExportModal();
  }

  exportToPDF(data: User[]): void {
    const doc = new jsPDF();
    
    doc.setFontSize(18);
    doc.text('Users Report', 14, 20);
    
    doc.setFontSize(10);
    doc.text(`Total Users: ${data.length}`, 14, 30);
    doc.text(`Generated: ${new Date().toLocaleString()}`, 14, 36);
    
    let yPos = 50;
    doc.setFontSize(12);
    
    data.forEach((user, index) => {
      if (yPos > 270) {
        doc.addPage();
        yPos = 20;
      }
      
      doc.setFont('helvetica', 'bold');
      doc.text(`${index + 1}. ${user.name}`, 14, yPos);
      
      doc.setFont('helvetica', 'normal');
      doc.setFontSize(10);
      doc.text(`Email: ${user.email}`, 20, yPos + 6);
      doc.text(`Role: ${user.role}`, 20, yPos + 12);
      doc.text(`Status: ${user.isApproved ? 'Approved' : 'Pending'}`, 20, yPos + 18);
      
      yPos += 28;
    });
    
    doc.save('users.pdf');
  }

  exportToExcel(data: User[]): void {
    const csv = [
      ['Name', 'Email', 'Role', 'Status', 'Created At'],
      ...data.map(u => [
        u.name,
        u.email,
        u.role,
        u.isApproved ? 'Approved' : 'Pending',
        new Date(u.createdAt).toLocaleDateString()
      ])
    ].map(row => row.join(',')).join('\n');
    
    const blob = new Blob([csv], { type: 'text/csv' });
    this.downloadFile(blob, 'users.csv');
  }

  exportToJSON(data: User[]): void {
    const json = JSON.stringify(data, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    this.downloadFile(blob, 'users.json');
  }

  exportToTXT(data: User[]): void {
    const text = data.map(u => 
      `${u.name} | ${u.email} | ${u.role} | ${u.isApproved ? 'Approved' : 'Pending'}`
    ).join('\n');
    
    const blob = new Blob([text], { type: 'text/plain' });
    this.downloadFile(blob, 'users.txt');
  }

  downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  getProfileImageUrl(user: User): string | null {
    return user.profileImageBase64 ? `data:image/jpeg;base64,${user.profileImageBase64}` : null;
  }
}
