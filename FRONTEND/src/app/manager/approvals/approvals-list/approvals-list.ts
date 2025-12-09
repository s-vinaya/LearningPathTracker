import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { ManagerApproval, ApprovalResponse } from '../../../services/manager-approval';
import { ApprovalDetailsDialog } from '../approval-details-dialog/approval-details-dialog';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-approvals-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './approvals-list.html',
  styleUrl: './approvals-list.css',
})
export class ApprovalsList implements OnInit {
  displayedColumns: string[] = ['type', 'employeeName', 'createdAt', 'status', 'actions'];
  approvals: ApprovalResponse[] = [];
  allApprovals: ApprovalResponse[] = [];
  loading = false;
  managerId: number;
  selectedFilter: string = 'all';

  constructor(
    private approvalService: ManagerApproval,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.managerId = this.authService.getCurrentUserId();
  }

  ngOnInit(): void {
    console.log('Loading approvals for manager ID:', this.managerId);
    this.loadAllApprovals();
  }

  loadAllApprovals(): void {
    this.loading = true;
    this.approvalService.getAllApprovals(this.managerId).subscribe({
      next: (data) => {
        console.log('All approvals loaded:', data);
        this.allApprovals = data;
        this.filterApprovals();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading approvals:', err);
        this.snackBar.open('Failed to load approvals', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  filterApprovals(): void {
    if (this.selectedFilter === 'all') {
      this.approvals = this.allApprovals;
    } else {
      this.approvals = this.allApprovals.filter(a => a.status.toLowerCase() === this.selectedFilter.toLowerCase());
    }
  }

  onFilterChange(filter: string): void {
    this.selectedFilter = filter;
    this.filterApprovals();
  }

  getTypeLabel(type: string): string {
    const labels: any = {
      'LPRequest': 'Learning Path Request',
      'ExtensionRequest': 'Deadline Extension',
      'ResourceRequest': 'Resource Request',
      'CertificateValidation': 'Certificate Validation'
    };
    return labels[type] || type;
  }

  openDetails(approval: ApprovalResponse): void {
    const dialogRef = this.dialog.open(ApprovalDetailsDialog, {
      width: '600px',
      data: { approval, managerId: this.managerId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadAllApprovals();
      }
    });
  }

  approve(approval: ApprovalResponse): void {
    this.submitDecision(approval.id, 'Approved', '');
  }

  reject(approval: ApprovalResponse): void {
    this.submitDecision(approval.id, 'Rejected', '');
  }

  private submitDecision(approvalId: number, status: string, comments: string): void {
    this.approvalService.submitApprovalDecision(this.managerId, {
      approvalId,
      status,
      reviewerComments: comments
    }).subscribe({
      next: () => {
        this.snackBar.open(`Approval ${status.toLowerCase()} successfully`, 'Close', { duration: 3000 });
        this.loadAllApprovals();
      },
      error: () => {
        this.snackBar.open('Failed to submit decision', 'Close', { duration: 3000 });
      }
    });
  }
}
