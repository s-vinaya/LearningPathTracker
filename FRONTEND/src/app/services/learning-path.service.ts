import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LearningPath {
  id: number;
  title: string;
  description: string;
  level: string;
  isActive: boolean;
  enrollmentCount?: number;
  modules?: any[];
  courses?: any[];
  createdAt?: string;
  estimatedHours?: number;
  createdBy?: string;
}

export interface Module {
  id: number;
  title: string;
  description: string;
  order: number;
}

@Injectable({
  providedIn: 'root'
})
export class LearningPathService {
  private apiUrl = 'https://localhost:7028/api/admin/learning-paths';

  constructor(private http: HttpClient) {}

  getAllLearningPaths(): Observable<LearningPath[]> {
    return this.http.get<LearningPath[]>(`${this.apiUrl}/with-courses`);
  }

  getStatistics(): Observable<any> {
    return this.http.get(`${this.apiUrl}/statistics`);
  }

  createLearningPath(path: any): Observable<LearningPath> {
    return this.http.post<LearningPath>(this.apiUrl, path);
  }

  updateLearningPath(id: number, path: any): Observable<LearningPath> {
    return this.http.put<LearningPath>(`${this.apiUrl}/${id}`, path);
  }

  deleteLearningPath(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  updateLearningPathModules(id: number, modules: any[]): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/modules`, modules);
  }
}
