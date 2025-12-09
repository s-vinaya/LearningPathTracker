import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ReportOverview {
  courseCompletions: number;
  courseCompletionsChange: number;
  activeLearnersCount: number;
  activeLearnersChange: number;
  avgCompletionWeeks: number;
  avgCompletionChange: number;
  satisfactionScore: number;
  satisfactionChange: number;
}

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = 'https://localhost:7028/api/reports';

  constructor(private http: HttpClient) {}

  getReportOverview(): Observable<ReportOverview> {
    return this.http.get<ReportOverview>(`${this.apiUrl}/overview`);
  }

  getDepartmentPerformance(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/department-performance`);
  }

  getTopLearners(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/top-learners`);
  }

  getPopularCourses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/popular-courses`);
  }

  getProgressOverTime(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/progress-over-time`);
  }
}
