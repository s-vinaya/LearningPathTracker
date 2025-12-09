import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ManagerApproval, ApprovalHistory } from '../../../services/manager-approval';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-approvals-history',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './approvals-history.html',
  styleUrl: './approvals-history.css',
})
export class ApprovalsHistory implements OnInit {
  displayedColumns: string[] = ['type', 'employeeName', 'status', 'reviewerComments', 'createdAt', 'reviewedAt'];
  allHistory: ApprovalHistory[] = [];
  filteredHistory: ApprovalHistory[] = [];
  loading = false;
  managerId: number;
  statusFilter = 'All';

  constructor(
    private approvalService: ManagerApproval,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.managerId = this.authService.getCurrentUserId();
  }

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.loading = true;
    this.approvalService.getApprovalHistory(this.managerId).subscribe({
      next: (data) => {
        this.allHistory = data;
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load history', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (this.statusFilter === 'All') {
      this.filteredHistory = this.allHistory;
    } else {
      this.filteredHistory = this.allHistory.filter(h => h.status === this.statusFilter);
    }
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

  getStatusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }
}
