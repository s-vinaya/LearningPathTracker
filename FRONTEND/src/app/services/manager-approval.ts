import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApprovalResponse {
  id: number;
  type: string;
  status: string;
  employeeName: string;
  payload: string;
  createdAt: string;
}

export interface ApprovalHistory {
  id: number;
  type: string;
  status: string;
  employeeName: string;
  payload: string;
  reviewerComments?: string;
  createdAt: string;
  reviewedAt?: string;
}

export interface ApprovalDecision {
  approvalId: number;
  status: string;
  reviewerComments: string;
}

@Injectable({
  providedIn: 'root',
})
export class ManagerApproval {
  private apiUrl = 'https://localhost:7028/api/manager';

  constructor(private http: HttpClient) { }

  getPendingApprovals(managerId: number): Observable<ApprovalResponse[]> {
    return this.http.get<ApprovalResponse[]>(`${this.apiUrl}/${managerId}/approvals/pending`);
  }

  getAllApprovals(managerId: number): Observable<ApprovalResponse[]> {
    return this.http.get<ApprovalResponse[]>(`${this.apiUrl}/${managerId}/approvals`);
  }

  getApprovalHistory(managerId: number): Observable<ApprovalHistory[]> {
    return this.http.get<ApprovalHistory[]>(`${this.apiUrl}/${managerId}/approvals/history`);
  }

  getApprovalById(managerId: number, approvalId: number): Observable<ApprovalHistory> {
    return this.http.get<ApprovalHistory>(`${this.apiUrl}/${managerId}/approvals/${approvalId}`);
  }

  submitApprovalDecision(managerId: number, decision: ApprovalDecision): Observable<any> {
    return this.http.post(`${this.apiUrl}/${managerId}/approvals/decision`, decision);
  }
}
