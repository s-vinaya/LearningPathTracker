import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Course {
  id: number;
  title: string;
  description: string;
  instructor: string;
  durationHours: number;
  category: string;
  isActive: boolean;
  createdAt: string;
  youTubeVideoId?: string;
  youTubeUrl?: string;
  thumbnailUrl?: string;
  videoDuration?: string;
  enrollmentCount: number;
}

export interface CourseStatistics {
  totalCourses: number;
  activeCourses: number;
  draftCourses: number;
  totalEnrollments: number;
}

export interface YouTubeVideo {
  videoId: string;
  title: string;
  description: string;
  channelTitle: string;
  thumbnailUrl: string;
  duration: string;
}

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private apiUrl = 'https://localhost:7028/api/admin/courses';
  private youtubeUrl = 'https://localhost:7028/api/youtube';

  constructor(private http: HttpClient) {}

  getAllCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(this.apiUrl);
  }

  getCourseStatistics(): Observable<CourseStatistics> {
    return this.http.get<CourseStatistics>(`${this.apiUrl}/statistics`);
  }

  createCourse(course: any): Observable<Course> {
    return this.http.post<Course>(this.apiUrl, course);
  }

  updateCourse(id: number, course: any): Observable<Course> {
    return this.http.put<Course>(`${this.apiUrl}/${id}`, course);
  }

  deleteCourse(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getYouTubeVideoInfo(url: string): Observable<YouTubeVideo> {
    return this.http.get<YouTubeVideo>(`${this.youtubeUrl}/video-info?url=${encodeURIComponent(url)}`);
  }

  bulkUploadCourses(youTubeUrls: string[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/bulk-upload`, { youTubeUrls });
  }

  getAllLearningPaths(): Observable<any[]> {
    return this.http.get<any[]>('https://localhost:7028/api/admin/learning-paths/with-courses');
  }
}
