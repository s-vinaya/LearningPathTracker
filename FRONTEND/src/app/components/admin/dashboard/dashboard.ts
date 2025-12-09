import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SidebarComponent } from '../sidebar/sidebar';
import { AdminDashboardService, DashboardSummary, EngagementData, RecentActivity } from '../../../services/admin-dashboard.service';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-dashboard',
  imports: [SidebarComponent, CommonModule, ChartModule, MatIconModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  standalone: true
})
export class AdminDashboardComponent implements OnInit {
  summary: DashboardSummary = {
    totalUsers: 0,
    activeCourses: 0,
    coursesCompleted: 0,
    certificatesIssued: 0,
    emailDeliveryRate: 0
  };
  
  engagementData: EngagementData = {
    courseCompletions: [],
    registrations: [],
    labels: []
  };
  
  chartData: any;
  chartOptions: any;
  
  recentActivities: RecentActivity[] = [];
  selectedRange: number = 7;
  loading: boolean = true;

  constructor(private dashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.loadDashboardData();
    this.initChart();
  }

  initChart(): void {
    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            usePointStyle: true,
            padding: 15
          }
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1
          }
        },
        x: {
          grid: {
            display: false
          }
        }
      }
    };
  }

  loadDashboardData(): void {
    this.loading = true;
    
    this.dashboardService.getDashboardSummary().subscribe({
      next: (data) => this.summary = data,
      error: (err) => console.error('Error loading summary:', err)
    });

    this.dashboardService.getEngagementData(this.selectedRange).subscribe({
      next: (data) => {
        this.engagementData = data;
        this.updateChart(data);
      },
      error: (err) => console.error('Error loading engagement:', err)
    });

    this.dashboardService.getRecentActivities().subscribe({
      next: (data) => {
        this.recentActivities = data.reverse();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading activities:', err);
        this.loading = false;
      }
    });
  }

  onRangeChange(range: number): void {
    this.selectedRange = range;
    this.dashboardService.getEngagementData(range).subscribe({
      next: (data) => {
        this.engagementData = data;
        this.updateChart(data);
      },
      error: (err) => console.error('Error loading engagement:', err)
    });
  }

  updateChart(data: EngagementData): void {
    this.chartData = {
      labels: data.labels,
      datasets: [
        {
          label: 'Course Completions',
          data: data.courseCompletions,
          borderColor: '#2196F3',
          backgroundColor: 'transparent',
          borderWidth: 3,
          tension: 0.4,
          fill: false,
          pointRadius: 4,
          pointBackgroundColor: '#2196F3'
        },
        {
          label: 'New Registrations',
          data: data.registrations,
          borderColor: '#4CAF50',
          backgroundColor: 'transparent',
          borderWidth: 3,
          tension: 0.4,
          fill: false,
          pointRadius: 4,
          pointBackgroundColor: '#4CAF50'
        }
      ]
    };
  }

  getActivityIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'user': '👤',
      'course': '📘',
      'path': '🎯',
      'certificate': '🏆',
      'system': '✓'
    };
    return icons[type.toLowerCase()] || '📌';
  }

  getActivityIconClass(type: string): string {
    const classes: { [key: string]: string } = {
      'user': 'green',
      'course': 'blue',
      'path': 'purple',
      'certificate': 'yellow',
      'system': 'green'
    };
    return classes[type.toLowerCase()] || 'blue';
  }

  getTotalCompletions(): number {
    return this.engagementData.courseCompletions.reduce((a, b) => a + b, 0);
  }

  getTotalRegistrations(): number {
    return this.engagementData.registrations.reduce((a, b) => a + b, 0);
  }
}
