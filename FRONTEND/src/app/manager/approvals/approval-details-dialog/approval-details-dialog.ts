import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ManagerApproval, ApprovalResponse } from '../../../services/manager-approval';

@Component({
  selector: 'app-approval-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule
  ],
  templateUrl: './approval-details-dialog.html',
  styleUrl: './approval-details-dialog.css',
})
export class ApprovalDetailsDialog {
  reviewerComments = '';
  payloadJson: any;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { approval: ApprovalResponse; managerId: number },
    private dialogRef: MatDialogRef<ApprovalDetailsDialog>,
    private approvalService: ManagerApproval,
    private snackBar: MatSnackBar
  ) {
    try {
      this.payloadJson = JSON.parse(data.approval.payload);
    } catch {
      this.payloadJson = data.approval.payload;
    }
  }

  approve(): void {
    this.submitDecision('Approved');
  }

  reject(): void {
    this.submitDecision('Rejected');
  }

  private submitDecision(status: string): void {
    this.approvalService.submitApprovalDecision(this.data.managerId, {
      approvalId: this.data.approval.id,
      status,
      reviewerComments: this.reviewerComments
    }).subscribe({
      next: () => {
        this.snackBar.open(`Approval ${status.toLowerCase()} successfully`, 'Close', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: () => {
        this.snackBar.open('Failed to submit decision', 'Close', { duration: 3000 });
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
