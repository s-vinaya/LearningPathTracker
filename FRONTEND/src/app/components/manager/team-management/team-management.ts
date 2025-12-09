import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ManagerSidebarComponent } from '../sidebar/manager-sidebar';
import { ManagerService } from '../../../services/manager';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

interface TeamMember {
  id: number;
  name: string;
  role: string;
  department: string;
  status: string;
  progress: number;
}

@Component({
  selector: 'app-team-management',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './team-management.component.html',
  styleUrls: ['../dashboard/manager-dashboard.css', './team-management.component.css']
})
export class TeamManagementComponent implements OnInit {
  teamMembers: TeamMember[] = [];
  dashboardStats: any = {};
  loading: boolean = true;
  error: string = '';
  showAddModal: boolean = false;
  showAssignModal: boolean = false;
  showDetailsModal: boolean = false;
  showProgressModal: boolean = false;
  selectedMember: any = null;
  employeeDetails: any = null;
  employeeProgress: any = null;
  learningPaths: any[] = [];
  showActionsDropdown: { [key: number]: boolean } = {};

  constructor(private managerService: ManagerService, private router: Router, private toast: ToastService, private confirm: ConfirmService) {}

  ngOnInit(): void {
    this.loadTeamMembers();
    this.loadDashboardStats();
    this.loadLearningPaths();
  }

  loadDashboardStats(): void {
    this.managerService.getDashboardStats().subscribe({
      next: (data) => {
        this.dashboardStats = data;
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        this.dashboardStats = {
          totalMembers: 0,
          active: 0,
          inTraining: 0,
          completedPaths: 0
        };
      }
    });
  }

  loadTeamMembers(): void {
    this.loading = true;
    this.managerService.getTeamMembersForManagement().subscribe({
      next: (data: any) => {
        console.log('Raw team members data:', data);
        this.teamMembers = data.map((member: any) => ({
          id: member.id || member.Id || member.userId || member.UserId,
          name: member.name || member.Name || member.fullName || member.FullName || `${member.firstName || ''} ${member.lastName || ''}`.trim(),
          role: member.role || member.Role || member.userRole || member.UserRole || member.jobTitle || member.JobTitle || 'Employee',
          department: member.department || member.Department || member.departmentName || member.DepartmentName,
          status: member.status || member.Status || (member.isActive ? 'Active' : 'Inactive'),
          progress: member.progress !== undefined ? member.progress : (member.Progress !== undefined ? member.Progress : 0)
        }));
        console.log('Mapped team members:', this.teamMembers);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading team members:', error);
        this.error = 'Failed to load team members';
        this.loading = false;
        this.teamMembers = [];
      }
    });
  }

  openAddModal(): void {
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  addTeamMember(): void {
    // TODO: Implement add team member functionality
    this.toast.show('Add team member functionality - Navigate to user management or implement invite', 'info');
    this.closeAddModal();
  }

  getStatusClass(status: string | undefined): string {
    return status?.toLowerCase() || '';
  }

  loadLearningPaths(): void {
    this.managerService.getLearningPaths().subscribe({
      next: (data) => {
        this.learningPaths = data;
      },
      error: (error) => {
        console.error('Error loading learning paths:', error);
      }
    });
  }

  openAssignModal(member: any): void {
    this.selectedMember = member;
    this.showAssignModal = true;
  }

  closeAssignModal(): void {
    this.showAssignModal = false;
    this.selectedMember = null;
  }

  assignLearningPath(learningPathId: number): void {
    if (!this.selectedMember) return;
    
    this.managerService.assignLearningPath(this.selectedMember.id, learningPathId).subscribe({
      next: () => {
        this.toast.show('Learning path assigned successfully!', 'success');
        this.closeAssignModal();
        this.loadTeamMembers();
      },
      error: (error) => {
        console.error('Error assigning learning path:', error);
        this.toast.show('Failed to assign learning path', 'error');
      }
    });
  }

  toggleActionsDropdown(memberId: number): void {
    this.showActionsDropdown[memberId] = !this.showActionsDropdown[memberId];
  }

  viewEmployeeDetails(member: any): void {
    this.selectedMember = member;
    this.managerService.getEmployeeDetails(member.id).subscribe({
      next: (data) => {
        this.employeeDetails = data;
        this.showDetailsModal = true;
      },
      error: (error) => {
        console.error('Error loading employee details:', error);
        this.toast.show('Failed to load employee details', 'error');
      }
    });
    this.showActionsDropdown[member.id] = false;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedMember = null;
    this.employeeDetails = null;
  }

  viewProgress(member: any): void {
    this.selectedMember = member;
    this.managerService.getEmployeeProgress(member.id).subscribe({
      next: (data) => {
        this.employeeProgress = data;
        this.showProgressModal = true;
      },
      error: (error) => {
        console.error('Error loading employee progress:', error);
        this.toast.show('Failed to load employee progress', 'error');
      }
    });
    this.showActionsDropdown[member.id] = false;
  }

  closeProgressModal(): void {
    this.showProgressModal = false;
    this.selectedMember = null;
    this.employeeProgress = null;
  }

  sendReminder(member: any): void {
    this.managerService.sendReminder(member.id).subscribe({
      next: (response) => {
        this.toast.show(`Reminder sent successfully to ${member.name}`, 'success');
      },
      error: (error) => {
        console.error('Error sending reminder:', error);
        this.toast.show('Failed to send reminder', 'error');
      }
    });
    this.showActionsDropdown[member.id] = false;
  }

  navigateToLearningPaths(): void {
    this.router.navigate(['/manager/learning-paths']);
  }
}