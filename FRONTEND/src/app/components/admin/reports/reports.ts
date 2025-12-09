import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar';
import { ReportService, ReportOverview } from '../../../services/report.service';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, ChartModule],
  templateUrl: './reports.html',
  styleUrl: './reports.css',
})
export class Reports implements OnInit {
  overview: ReportOverview | null = null;
  departments: any[] = [];
  topLearners: any[] = [];
  popularCourses: any[] = [];
  progressData: any[] = [];
  
  selectedPeriod: string = 'last30';
  loading: boolean = true;
  progressChartData: any;
  progressChartOptions: any;
  popularCoursesData: any;
  popularCoursesOptions: any;

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.loadReports();
    this.initCharts();
  }

  loadReports(): void {
    this.loading = true;
    
    this.reportService.getReportOverview().subscribe({
      next: (data) => {
        this.overview = data;
      },
      error: (err) => console.error('Error loading overview:', err)
    });

    this.reportService.getDepartmentPerformance().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => console.error('Error loading departments:', err)
    });

    this.reportService.getTopLearners().subscribe({
      next: (data) => {
        this.topLearners = data;
      },
      error: (err) => console.error('Error loading top learners:', err)
    });

    this.reportService.getPopularCourses().subscribe({
      next: (data) => {
        this.popularCourses = data;
        this.updatePopularCoursesChart(data);
      },
      error: (err) => console.error('Error loading popular courses:', err)
    });

    this.reportService.getProgressOverTime().subscribe({
      next: (data) => {
        this.progressData = data;
        this.updateProgressChart(data);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading progress:', err);
        this.loading = false;
      }
    });
  }

  initCharts(): void {
    this.progressChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false }
      },
      scales: {
        y: { beginAtZero: true }
      }
    };

    this.popularCoursesOptions = {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: 'y',
      plugins: {
        legend: { display: false }
      }
    };
  }

  updateProgressChart(data: any[]): void {
    const labels = data.map(d => new Date(d.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
    const enrollments = data.map(d => d.enrollments);
    const completions = data.map(d => d.completions);

    this.progressChartData = {
      labels,
      datasets: [
        {
          label: 'Enrollments',
          data: enrollments,
          borderColor: '#7c3aed',
          backgroundColor: 'rgba(124, 58, 237, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          label: 'Completions',
          data: completions,
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  updatePopularCoursesChart(data: any[]): void {
    const labels = data.map(c => c.title);
    const enrollments = data.map(c => c.enrollments);

    this.popularCoursesData = {
      labels,
      datasets: [{
        data: enrollments,
        backgroundColor: ['#7c3aed', '#3b82f6', '#10b981', '#f59e0b', '#ef4444']
      }]
    };
  }

  getChangeIcon(change: number): string {
    return change >= 0 ? '↑' : '↓';
  }

  getChangeClass(change: number): string {
    return change >= 0 ? 'positive' : 'negative';
  }

  exportReport(): void {
    const data = [
      ['Reports & Analytics'],
      [''],
      ['Overview'],
      ['Course Completions', this.overview?.courseCompletions || 0],
      ['Active Learners', this.overview?.activeLearnersCount || 0],
      ['Avg. Completion Time (weeks)', this.overview?.avgCompletionWeeks || 0],
      ['Satisfaction Score', this.overview?.satisfactionScore || 0],
      [''],
      ['Department Performance'],
      ['Department', 'Total Users', 'Active Learners', 'Completions', 'Avg. Score', 'Progress'],
      ...this.departments.map(d => [d.department, d.totalUsers, d.activeLearnersCount, d.completions, d.avgScore.toFixed(0) + '%', d.progress.toFixed(0) + '%']),
      [''],
      ['Top Learners'],
      ['Rank', 'Name', 'Department', 'Completions'],
      ...this.topLearners.map((l, i) => [i + 1, l.name, l.department, l.completions]),
      [''],
      ['Popular Courses'],
      ['Course', 'Enrollments'],
      ...this.popularCourses.map(c => [c.title, c.enrollments])
    ];

    const csv = data.map(row => row.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `report-${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
