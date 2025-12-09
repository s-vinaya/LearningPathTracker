import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApprovalsRoutingModule } from './approvals-routing-module';
import { ApprovalsList } from './approvals-list/approvals-list';
import { ApprovalsHistory } from './approvals-history/approvals-history';
import { ApprovalDetailsDialog } from './approval-details-dialog/approval-details-dialog';

@NgModule({
  imports: [
    CommonModule,
    ApprovalsRoutingModule,
    ApprovalsList,
    ApprovalsHistory,
    ApprovalDetailsDialog
  ]
})
export class ApprovalsModule { }
