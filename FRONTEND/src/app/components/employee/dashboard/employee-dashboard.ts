import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent, RouterModule, MatIconModule],
  templateUrl: './employee-dashboard.html',
  styleUrls: ['./employee-dashboard.css']
})
export class EmployeeDashboardComponent implements OnInit {
  stats: any = {};
  loading = true;
  learningPaths: any[] = [];
  recentProgress: any[] = [];
  badges: any[] = [];
  certificates: any[] = [];
  leaderboard: any = { top3: [], currentUser: null };
  streaks: any = {};
  userId: number = 0;
  private apiUrl = 'https://localhost:7028/api';

  constructor(
    private employeeService: EmployeeService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.userId = user.id;
    this.loadAllDashboardData();
  }

  loadAllDashboardData() {
    this.loading = true;

    this.employeeService.getAssignedLearningPaths().subscribe({
      next: (assignedPaths) => {
        this.stats.assignedPaths = assignedPaths.length;
      },
      error: (err) => console.error('Error loading learning paths:', err)
    });

    this.employeeService.getProgress().subscribe({
      next: (data) => {
        const totalMinutes = data.totalHours || 0;
        this.stats.completedModules = data.completedModules || 0;
        this.stats.totalModules = data.totalModules || 0;
        this.stats.weeklyHours = totalMinutes < 60 ? `${totalMinutes}m` : `${Math.round(totalMinutes / 60)}h`;
        this.stats.overallProgress = data.totalModules > 0 ? Math.round((data.completedModules / data.totalModules) * 100) : 0;
      },
      error: (err) => console.error('Error loading progress:', err)
    });

    this.employeeService.getCertificates().subscribe({
      next: (data) => {
        this.certificates = data.slice(0, 3);
        this.stats.certificates = data.length;
      },
      error: (err) => console.error('Error loading certificates:', err)
    });

    this.employeeService.getEnrolledCourses().subscribe({
      next: (data) => {
        this.stats.enrolledCourses = data.length;
        this.recentProgress = data.slice(0, 5).map((course: any) => ({
          title: course.title,
          description: `Progress: ${course.progress}%`,
          date: course.enrolledAt,
          progress: course.progress
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading courses:', err);
        this.loading = false;
      }
    });

    this.loadLeaderboard();
    this.loadStreaks();
  }

  loadLeaderboard() {
    this.http.get(`${this.apiUrl}/Leaderboard/employee/${this.userId}`).subscribe({
      next: (data: any) => {
        this.leaderboard = data;
      },
      error: (err) => console.error('Error loading leaderboard:', err)
    });
  }

  loadStreaks() {
    this.http.get(`${this.apiUrl}/Leaderboard/streaks/${this.userId}`).subscribe({
      next: (data: any) => {
        this.streaks = data;
      },
      error: (err) => console.error('Error loading streaks:', err)
    });
  }

  getCircularProgress(percent: number) {
    const radius = 70;
    const circumference = 2 * Math.PI * radius;
    const offset = circumference - (percent / 100) * circumference;
    return { circumference, offset };
  }

  getInitials(name: string): string {
    return name?.split(' ').map(n => n[0]).join('').toUpperCase() || '?';
  }


}
