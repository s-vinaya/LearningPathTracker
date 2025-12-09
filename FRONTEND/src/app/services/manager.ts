import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private apiUrl = 'https://localhost:7028/api/manager';

  constructor(private http: HttpClient) {}

  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard-stats`);
  }

  getTeamMembers(): Observable<any> {
    return this.http.get('https://localhost:7028/api/manager/assignments/employees');
  }

  getLearningPaths(): Observable<any> {
    return this.http.get('https://localhost:7028/api/admin/learning-paths/with-courses');
  }

  getReports(): Observable<any> {
    return this.http.get(`${this.apiUrl}/reports`);
  }

  getWeeklyHours(): Observable<any> {
    return this.http.get(`${this.apiUrl}/weekly-hours`);
  }

  getManagerProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/profile`);
  }

  assignLearningPath(userId: number, learningPathId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/enrollment/assign-learning-path`, { userId, learningPathId });
  }

  getPendingApprovals(): Observable<any> {
    return this.http.get(`${this.apiUrl}/approvals`);
  }

  getEmployeeDetails(employeeId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/employee-details/${employeeId}`);
  }

  getEmployeeProgress(employeeId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/employee-progress/${employeeId}`);
  }

  sendReminder(employeeId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-reminder/${employeeId}`, {});
  }

  bulkAssignLearningPaths(assignment: any): Observable<any> {
    if (assignment.employeeIds && assignment.employeeIds.length > 0) {
      return this.http.post('https://localhost:7028/api/manager/assignments/assign', assignment);
    } else {
      return this.http.post('https://localhost:7028/api/manager/assignments/bulk-assign', assignment);
    }
  }

  removeAssignment(assignmentId: number): Observable<any> {
    return this.http.delete(`https://localhost:7028/api/manager/assignments/${assignmentId}`);
  }

  getTeamMembersForManagement(): Observable<any> {
    return this.http.get('https://localhost:7028/api/manager/assignments/employees');
  }

  getUpcomingDeadlines(days: number = 7): Observable<any> {
    return this.http.get(`https://localhost:7028/api/manager/dashboard/deadlines?days=${days}`);
  }

  getManagerId(): number {
    const userId = localStorage.getItem('userId');
    return userId ? parseInt(userId) : 0;
  }
}
