import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Notification {
  id: number;
  title: string;
  message: string;
  type: string;
  createdAt: string;
  isRead: boolean;
  recipients: string;
  recipientCount: number;
  deliveredCount: number;
  openedCount: number;
  clickedCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = 'https://localhost:7028/api/admin/notifications';

  constructor(private http: HttpClient) {}

  getAllNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(this.apiUrl);
  }

  getNotificationStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/statistics`);
  }

  createNotification(notification: any): Observable<Notification> {
    return this.http.post<Notification>(this.apiUrl, notification);
  }

  deleteNotification(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getNotificationById(id: number): Observable<Notification> {
    return this.http.get<Notification>(`${this.apiUrl}/${id}`);
  }

  getDeliveryStatus(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}/delivery-status`);
  }
}

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {
  private apiUrl = 'https://localhost:7028/api/leaderboard';

  constructor(private http: HttpClient) {}

  getTop3(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/top3`);
  }
}
