import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApprovalsList } from './approvals-list/approvals-list';
import { ApprovalsHistory } from './approvals-history/approvals-history';

const routes: Routes = [
  { path: '', component: ApprovalsList },
  { path: 'history', component: ApprovalsHistory }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApprovalsRoutingModule { }
