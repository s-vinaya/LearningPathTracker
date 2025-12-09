import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './courses.html',
  styleUrl: './courses.css'
})
export class CoursesComponent implements OnInit {
  availableCourses: any[] = [];
  enrolledCourses: any[] = [];
  loading = true;
  showEnrolled = false;
  errorMessage = '';

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.loadAvailableCourses();
    this.loadEnrolledCourses();
  }

  loadAvailableCourses() {
    this.employeeService.getAllAvailableCourses().subscribe({
      next: (data) => {
        this.availableCourses = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading courses:', err);
        this.loading = false;
      }
    });
  }

  loadEnrolledCourses() {
    this.employeeService.getEnrolledCourses().subscribe({
      next: (data) => {
        console.log('Enrolled courses:', data);
        this.enrolledCourses = Array.isArray(data) ? data : [];
      },
      error: (err) => {
        console.error('Error loading enrolled courses:', err);
        this.enrolledCourses = [];
        if (err.status === 401) {
          this.errorMessage = 'Authentication error. Please log in again.';
        }
      }
    });
  }

  enrollInCourse(courseId: number) {
    this.employeeService.enrollInCourse(courseId).subscribe({
      next: (response) => {
        this.toast.show('Successfully enrolled in the course!', 'success');
        this.loadAvailableCourses();
        this.loadEnrolledCourses();
      },
      error: (err) => {
        const message = err.error?.message || 'Failed to enroll in course';
        this.toast.show(message, 'error');
        console.error('Error enrolling:', err);
      }
    });
  }

  viewCourse(courseId: number) {
    this.router.navigate(['/employee/course-viewer', courseId]);
  }
}
