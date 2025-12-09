import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { SidebarComponent } from '../sidebar/sidebar';
import { NotificationService, Notification, LeaderboardService } from '../../../services/notification.service';

@Component({
  selector: 'app-send-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, MatIconModule],
  templateUrl: './send-notifications.html',
  styleUrl: './send-notifications.css',
})
export class SendNotifications implements OnInit {
  notifications: Notification[] = [];
  loading: boolean = true;
  selectedStatus: string = 'all';
  stats: any = { totalSent: 0, delivered: 0, openRate: 0, clickRate: 0 };
  showComposeModal: boolean = false;
  showViewModal: boolean = false;
  showDeliveryModal: boolean = false;
  showTop3Modal: boolean = false;
  selectedNotification: Notification | null = null;
  deliveryStatus: any = null;
  top3Performers: any[] = [];
  showToast: boolean = false;
  toastMessage: string = '';
  newNotification = {
    title: '',
    message: '',
    type: 'info',
    recipients: 'all'
  };

  constructor(
    private notificationService: NotificationService,
    private leaderboardService: LeaderboardService
  ) {}

  ngOnInit(): void {
    this.loadNotifications();
    this.loadStats();
  }

  loadNotifications(): void {
    this.loading = true;
    this.notificationService.getAllNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading notifications:', err);
        this.loading = false;
      }
    });
  }

  loadStats(): void {
    this.notificationService.getNotificationStats().subscribe({
      next: (data) => this.stats = data,
      error: (err) => console.error('Error loading stats:', err)
    });
  }

  get totalSent(): number {
    return this.stats.totalSent || 0;
  }

  get deliveryRate(): string {
    return this.stats.deliveryRate ? `${this.stats.deliveryRate}%` : '0%';
  }

  openComposeModal(): void {
    this.newNotification = {
      title: '',
      message: '',
      type: 'info',
      recipients: 'all'
    };
    this.showComposeModal = true;
  }

  closeComposeModal(): void {
    this.showComposeModal = false;
  }

  sendNotification(): void {
    if (!this.newNotification.title || !this.newNotification.message) {
      this.showToastMessage('Please fill in all required fields');
      return;
    }

    this.notificationService.createNotification(this.newNotification).subscribe({
      next: () => {
        this.closeComposeModal();
        this.showToastMessage('Email sent successfully!');
        this.loadNotifications();
        this.loadStats();
      },
      error: (err) => {
        console.error('Error sending notification:', err);
        this.showToastMessage('Failed to send notification');
      }
    });
  }

  deleteNotification(id: number): void {
    if (confirm('Are you sure you want to delete this notification?')) {
      this.notificationService.deleteNotification(id).subscribe({
        next: () => {
          this.showToastMessage('Notification deleted successfully');
          this.loadNotifications();
          this.loadStats();
        },
        error: (err) => {
          console.error('Error deleting notification:', err);
          this.showToastMessage('Failed to delete notification');
        }
      });
    }
  }

  viewNotification(id: number): void {
    this.notificationService.getNotificationById(id).subscribe({
      next: (notification) => {
        this.selectedNotification = notification;
        this.showViewModal = true;
      },
      error: (err) => console.error('Error loading notification:', err)
    });
  }

  closeViewModal(): void {
    this.showViewModal = false;
    this.selectedNotification = null;
  }

  viewDeliveryStatus(id: number): void {
    this.notificationService.getDeliveryStatus(id).subscribe({
      next: (status) => {
        this.deliveryStatus = status;
        this.showDeliveryModal = true;
      },
      error: (err) => console.error('Error loading delivery status:', err)
    });
  }

  closeDeliveryModal(): void {
    this.showDeliveryModal = false;
    this.deliveryStatus = null;
  }

  sendCourseReminder(): void {
    this.newNotification = {
      title: 'Course Reminder',
      message: 'You have pending courses to complete. Please log in to your dashboard and continue your learning journey.',
      type: 'warning',
      recipients: 'employees'
    };
    this.showComposeModal = true;
  }

  sendNewCourseAlert(): void {
    this.newNotification = {
      title: 'New Course Available',
      message: 'A new course has been added to the platform. Check it out and enroll if you are interested!',
      type: 'info',
      recipients: 'all'
    };
    this.showComposeModal = true;
  }

  sendAchievementCongrats(): void {
    this.leaderboardService.getTop3().subscribe({
      next: (data) => {
        this.top3Performers = data;
        this.showTop3Modal = true;
      },
      error: (err) => {
        console.error('Error loading top performers:', err);
        this.showToastMessage('Failed to load top performers');
      }
    });
  }

  closeTop3Modal(): void {
    this.showTop3Modal = false;
    this.top3Performers = [];
  }

  confirmSendCongrats(): void {
    const names = this.top3Performers.map((p, i) => `${i + 1}. ${p.name}`).join(', ');
    this.newNotification = {
      title: 'Achievement Congratulations 🏆',
      message: `Congratulations to our top 3 performers: ${names}! Your dedication and hard work are truly inspiring. Keep up the excellent work!`,
      type: 'success',
      recipients: 'all'
    };
    this.closeTop3Modal();
    this.showComposeModal = true;
  }

  showToastMessage(message: string): void {
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  getTimeAgo(date: string): string {
    const past = new Date(date);
    const now = new Date();
    const diffMs = now.getTime() - past.getTime();
    if (diffMs < 0) return 'Just now';
    
    const diffMinutes = Math.floor(diffMs / 60000);
    if (diffMinutes < 1) return 'Just now';
    if (diffMinutes < 60) return `${diffMinutes} minute${diffMinutes > 1 ? 's' : ''} ago`;
    
    const diffHours = Math.floor(diffMinutes / 60);
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    
    const diffDays = Math.floor(diffHours / 24);
    if (diffDays < 30) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    
    const diffMonths = Math.floor(diffDays / 30);
    return `${diffMonths} month${diffMonths > 1 ? 's' : ''} ago`;
  }
}
