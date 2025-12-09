import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ManagerDashboard {
  totalEmployees: number;
  activeLearningPaths: number;
  completedLearningPaths: number;
  pendingApprovals: number;
  upcomingDeadlines: UpcomingDeadline[];
}

export interface UpcomingDeadline {
  assignmentId: number;
  employeeId: number;
  employeeName: string;
  learningPathTitle: string;
  dueDate: string;
  progressPercent: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class ManagerDashboardService {
  private apiUrl = 'https://localhost:7028/api';

  constructor(private http: HttpClient) {}

  getManagerDashboard(managerId: number): Observable<ManagerDashboard> {
    return this.http.get<ManagerDashboard>(`${this.apiUrl}/manager/${managerId}/dashboard`);
  }
}
