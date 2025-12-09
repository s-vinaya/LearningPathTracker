import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private apiUrl = 'https://localhost:7028/api/manager';

  constructor(private http: HttpClient) {}

  getPendingEnrollmentRequests(): Observable<any> {
    return this.http.get(`${this.apiUrl}/approvals`);
  }

  approveEnrollmentRequest(requestId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/approvals/${requestId}/approve`, {});
  }

  rejectEnrollmentRequest(requestId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/approvals/${requestId}/reject`, {});
  }

  getTeamMembersForManagement(): Observable<any> {
    return this.http.get(`${this.apiUrl}/team-members`);
  }

  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard-stats`);
  }

  getLearningPaths(): Observable<any> {
    return this.http.get(`${this.apiUrl}/learning-paths`);
  }

  assignLearningPath(employeeId: number, learningPathId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign-learning-path`, { employeeId, learningPathId });
  }

  getEmployeeDetails(employeeId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/employees/${employeeId}/details`);
  }

  getEmployeeProgress(employeeId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/employees/${employeeId}/progress`);
  }

  sendReminder(employeeId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/employees/${employeeId}/reminder`, {});
  }
}