import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagerSidebarComponent } from '../sidebar/manager-sidebar';
import { ManagerService } from '../../../services/manager.service';

@Component({
  selector: 'app-manager-approvals',
  standalone: true,
  imports: [CommonModule, ManagerSidebarComponent],
  templateUrl: './approvals.html',
  styleUrl: './approvals.css'
})
export class ManagerApprovalsComponent implements OnInit {
  pendingRequests: any[] = [];
  loading = true;

  constructor(private managerService: ManagerService) {}

  ngOnInit() {
    this.loadPendingRequests();
  }

  loadPendingRequests() {
    this.managerService.getPendingEnrollmentRequests().subscribe({
      next: (data: any) => {
        this.pendingRequests = (data && data.approvals) ? data.approvals : [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading requests:', err);
        this.loading = false;
      }
    });
  }

  approveRequest(requestId: number) {
    if (confirm('Approve this enrollment request?')) {
      this.managerService.approveEnrollmentRequest(requestId).subscribe({
        next: () => {
          alert('Request approved successfully!');
          this.loadPendingRequests();
        },
        error: (err) => console.error('Error approving request:', err)
      });
    }
  }

  rejectRequest(requestId: number) {
    if (confirm('Reject this enrollment request?')) {
      this.managerService.rejectEnrollmentRequest(requestId).subscribe({
        next: () => {
          alert('Request rejected.');
          this.loadPendingRequests();
        },
        error: (err) => console.error('Error rejecting request:', err)
      });
    }
  }
}