import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ManagerSidebarComponent } from '../sidebar/manager-sidebar';
import { ManagerService } from '../../../services/manager';

interface TeamMember {
  name: string;
  role: string;
  department: string;
  status: string;
  progress: number;
}

@Component({
  selector: 'app-manager-dashboard',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './manager-dashboard.component.html',
  styleUrl: './manager-dashboard.css',
})
export class ManagerDashboardComponent implements OnInit {
  dashboardStats: any = {
    totalMembers: 0,
    active: 0,
    inTraining: 0,
    completedPaths: 0,
    activePaths: 0,
    completionRate: 0,
    weeklyHours: 0
  };
  upcomingDeadlines: any[] = [];
  filteredDeadlines: any[] = [];
  pendingApprovals: number = 0;
  loading: boolean = true;
  error: string = '';
  selectedDaysFilter: number = 7;
  daysFilterOptions = [3, 7, 14, 30];
  teamLeaderboard: any[] = [];
  managerId: number = 0;
  private apiUrl = 'https://localhost:7028/api';

  constructor(
    private router: Router, 
    private managerService: ManagerService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.managerId = user.id;
    this.loadDashboardStats();
    this.loadApprovals();
    this.loadUpcomingDeadlines();
    this.loadTeamLeaderboard();
  }

  loadDashboardStats(): void {
    this.managerService.getDashboardStats().subscribe({
      next: (data) => {
        console.log('Dashboard stats loaded:', data);
        this.dashboardStats = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        this.error = 'Failed to load dashboard data';
        this.loading = false;
      }
    });
  }

  loadApprovals(): void {
    this.managerService.getPendingApprovals().subscribe({
      next: (data: any) => {
        this.pendingApprovals = data.count || 0;
      },
      error: (error) => {
        console.error('Error loading approvals:', error);
        this.pendingApprovals = 0;
      }
    });
  }

  loadUpcomingDeadlines(): void {
    this.managerService.getUpcomingDeadlines(this.selectedDaysFilter).subscribe({
      next: (data) => {
        this.upcomingDeadlines = data;
        this.filteredDeadlines = data;
      },
      error: (error) => {
        console.error('Error loading deadlines:', error);
        this.upcomingDeadlines = [];
        this.filteredDeadlines = [];
      }
    });
  }

  onDaysFilterChange(days: number): void {
    this.selectedDaysFilter = days;
    this.loadUpcomingDeadlines();
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  getUrgencyClass(daysRemaining: number): string {
    if (daysRemaining <= 1) return 'urgent';
    if (daysRemaining <= 3) return 'warning';
    return 'normal';
  }

  loadTeamLeaderboard(): void {
    this.http.get(`${this.apiUrl}/Leaderboard/manager/${this.managerId}`).subscribe({
      next: (data: any) => {
        this.teamLeaderboard = data;
      },
      error: (err) => console.error('Error loading team leaderboard:', err)
    });
  }

  getInitials(name: string): string {
    return name?.split(' ').map(n => n[0]).join('').toUpperCase() || '?';
  }
}
