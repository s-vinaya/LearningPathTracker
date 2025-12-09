import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = 'https://localhost:7028/api/employee';
  
  private getUserId(): number {
    const user = localStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      return userData.id || userData.Id || userData.userId || userData.UserId || 1;
    }
    return 1;
  }

  constructor(private http: HttpClient) {}

  getAssignedLearningPaths(): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/learning-paths`);
  }

  getLearningPathModules(pathId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/learning-paths/${pathId}/modules`);
  }

  unlockModule(moduleId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/modules/${moduleId}/unlock`, {});
  }

  getQuiz(moduleId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/modules/${moduleId}/quiz`);
  }

  checkQuizAttempts(quizId: number): Observable<any> {
    return this.http.get(`https://localhost:7028/api/admin/quizzes/${quizId}/check-attempts`);
  }

  submitQuiz(quizId: number, answers: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/quizzes/${quizId}/submit`, answers);
  }

  getNotes(moduleId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/modules/${moduleId}/notes`);
  }

  saveNote(moduleId: number, note: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/modules/${moduleId}/notes`, note);
  }

  deleteNote(noteId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/notes/${noteId}`);
  }

  getProgress(): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/progress`);
  }

  getWeeklyHours(): Observable<any> {
    return this.http.get(`${this.apiUrl}/weekly-hours`);
  }

  setWeeklyHours(hours: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/weekly-hours`, { hours });
  }

  getCertificates(): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/certificates`);
  }

  downloadCertificate(certificateId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/certificates/${certificateId}/download`, { responseType: 'blob' });
  }

  previewCertificate(certificateId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/certificates/${certificateId}/preview`, { responseType: 'blob' });
  }

  getBadges(): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/badges`);
  }

  getAchievements(): Observable<any> {
    return this.http.get(`${this.apiUrl}/achievements`);
  }

  getStreaks(): Observable<any> {
    return this.http.get(`${this.apiUrl}/streaks`);
  }

  getFlashcards(moduleId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/modules/${moduleId}/flashcards`);
  }

  getTips(): Observable<any> {
    return this.http.get(`${this.apiUrl}/tips`);
  }

  getDashboardStats(): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/dashboard`);
  }

  getAllAvailableCourses(): Observable<any> {
    return this.http.get(`${this.apiUrl}/courses/available`);
  }

  getEnrolledCourses(): Observable<any> {
    return this.http.get(`${this.apiUrl}/courses/enrolled`);
  }

  getAllAvailableLearningPaths(): Observable<any> {
    const timestamp = new Date().getTime();
    return this.http.get(`https://localhost:7028/api/admin/learning-paths/with-courses?t=${timestamp}`);
  }

  enrollInCourse(courseId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/courses/${courseId}/enroll`, {});
  }

  enrollInLearningPath(learningPathId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/learning-paths/${learningPathId}/enroll`, {});
  }

  getCourseDetails(courseId: number): Observable<any> {
    const userId = this.getUserId();
    return this.http.get(`${this.apiUrl}/${userId}/courses/${courseId}`);
  }

  updateVideoProgress(courseId: number, progress: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/courses/${courseId}/progress`, { progress });
  }

  generateCertificate(courseId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/courses/${courseId}/certificate`, {});
  }

  getLearningSchedule(): Observable<any> {
    return this.http.get(`${this.apiUrl}/learning-schedule`);
  }

  setLearningSchedule(startTime: string, endTime: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/learning-schedule`, { startTime, endTime });
  }

  getLearningPathFinalQuiz(learningPathId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/learning-paths/${learningPathId}/final-quiz`);
  }
}
