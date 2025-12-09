import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-course-request',
  standalone: true,
  imports: [CommonModule, FormsModule, EmployeeSidebarComponent],
  templateUrl: './course-request.html',
  styleUrls: ['./course-request.css']
})
export class CourseRequest implements OnInit {
  myRequests: any[] = [];
  courseName: string = '';
  reason: string = '';
  loading: boolean = false;
  userId: number = 0;
  showModal: boolean = false;
  private apiUrl = 'https://localhost:7028/api';

  constructor(private http: HttpClient, private toast: ToastService) {}

  ngOnInit() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.userId = user.id;
    this.loadMyRequests();
  }

  loadMyRequests() {
    this.http.get(`${this.apiUrl}/EmployeeCourseRequest/my-requests/${this.userId}`).subscribe({
      next: (data: any) => {
        this.myRequests = data;
      },
      error: (err) => console.error('Error loading requests:', err)
    });
  }

  openRequestModal() {
    this.showModal = true;
    this.courseName = '';
    this.reason = '';
  }

  closeModal() {
    this.showModal = false;
  }

  submitRequest() {
    if (!this.courseName.trim() || !this.reason.trim()) {
      this.toast.show('Please enter course name and provide a reason', 'error');
      return;
    }

    this.loading = true;
    const request = {
      employeeId: this.userId,
      courseName: this.courseName,
      reason: this.reason
    };

    this.http.post(`${this.apiUrl}/EmployeeCourseRequest/request-custom`, request).subscribe({
      next: () => {
        this.loading = false;
        this.closeModal();
        this.toast.show('Course request submitted successfully!', 'success');
        this.loadMyRequests();
      },
      error: (err) => {
        this.loading = false;
        console.error('Error submitting request:', err);
        this.toast.show('Failed to submit request', 'error');
      }
    });
  }

  getStatusClass(status: string): string {
    return status.toLowerCase();
  }

  getCourseNameFromDetails(details: string): string {
    const match = details.match(/Course: ([^|]+)/);
    return match ? match[1] : 'N/A';
  }

  getReasonFromDetails(details: string): string {
    const match = details.match(/Reason: (.+)/);
    return match ? match[1] : 'N/A';
  }
}
