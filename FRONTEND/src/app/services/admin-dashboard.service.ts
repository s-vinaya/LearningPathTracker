import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardSummary {
  totalUsers: number;
  activeCourses: number;
  coursesCompleted: number;
  certificatesIssued: number;
  emailDeliveryRate: number;
}

export interface EngagementData {
  courseCompletions: number[];
  registrations: number[];
  labels: string[];
}

export interface RecentActivity {
  type: string;
  text: string;
  timeAgo: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  private apiUrl = 'https://localhost:7028/api/admin';

  constructor(private http: HttpClient) {}

  getDashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.apiUrl}/dashboard/summary`);
  }

  getEngagementData(range: number = 7): Observable<EngagementData> {
    return this.http.get<EngagementData>(`${this.apiUrl}/dashboard/engagement?range=${range}`);
  }

  getRecentActivities(): Observable<RecentActivity[]> {
    return this.http.get<RecentActivity[]>(`${this.apiUrl}/dashboard/recent-activities`);
  }
}
