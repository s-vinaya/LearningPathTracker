import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Department {
  id: number;
  name: string;
  description: string;
  managerId?: number;
  managerName?: string;
  createdAt: string;
  isActive: boolean;
  employeeCount?: number;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private apiUrl = 'https://localhost:7028/api/departments';

  constructor(private http: HttpClient) {}

  getAllDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(this.apiUrl);
  }

  getDepartmentById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createDepartment(department: any): Observable<Department> {
    return this.http.post<Department>(this.apiUrl, department);
  }

  updateDepartment(id: number, department: any): Observable<Department> {
    return this.http.put<Department>(`${this.apiUrl}/${id}`, department);
  }

  deleteDepartment(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  assignManager(departmentId: number, managerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${departmentId}/assign-manager`, managerId);
  }

  assignEmployee(departmentId: number, userId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${departmentId}/assign-employee`, { userId });
  }

  removeEmployee(departmentId: number, userId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${departmentId}/remove-employee/${userId}`, {});
  }

  changeEmployeeDepartment(userId: number, newDepartmentId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/change-employee-department`, { userId, newDepartmentId });
  }

  getDepartmentEmployees(departmentId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${departmentId}/employees`);
  }

  getUnassignedEmployees(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/unassigned-employees`);
  }
}
