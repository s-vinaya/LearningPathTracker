import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar';
import { DepartmentService, Department } from '../../../services/department.service';
import { UserService, User } from '../../../services/user.service';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

@Component({
  selector: 'app-manage-departments',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent],
  templateUrl: './manage-departments.html',
  styleUrl: './manage-departments.css',
})
export class ManageDepartments implements OnInit {
  departments: Department[] = [];
  users: User[] = [];
  unassignedUsers: User[] = [];
  managers: User[] = [];
  loading: boolean = true;
  showCreateModal: boolean = false;
  showEditModal: boolean = false;
  showAssignModal: boolean = false;
  showManageEmployeesModal: boolean = false;
  selectedDepartment: any = null;
  departmentEmployees: any[] = [];
  departmentForm: any = {
    name: '',
    description: '',
    managerId: null,
    isActive: true
  };
  assignForm: any = {
    userId: null
  };

  constructor(
    private departmentService: DepartmentService,
    private userService: UserService,
    private toast: ToastService,
    private confirm: ConfirmService
  ) {}

  ngOnInit(): void {
    this.loadDepartments();
    this.loadUsers();
  }

  loadDepartments(): void {
    this.loading = true;
    this.departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.departments = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading departments:', err);
        this.loading = false;
      }
    });
  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data.filter(u => u.role === 'Employee');
        this.managers = data.filter(u => u.role === 'Manager' || u.role === 'Admin');
      },
      error: (err) => console.error('Error loading users:', err)
    });
    this.loadUnassignedUsers();
  }

  loadUnassignedUsers(): void {
    this.departmentService.getUnassignedEmployees().subscribe({
      next: (data) => {
        this.unassignedUsers = data;
      },
      error: (err) => console.error('Error loading unassigned users:', err)
    });
  }

  openCreateModal(): void {
    this.departmentForm = {
      name: '',
      description: '',
      managerId: null,
      isActive: true
    };
    this.showCreateModal = true;
  }

  openEditModal(department: Department): void {
    this.departmentForm = { ...department };
    this.showEditModal = true;
  }

  openAssignModal(department: Department): void {
    this.selectedDepartment = department;
    this.assignForm.userId = null;
    this.showAssignModal = true;
  }

  openManageEmployeesModal(department: Department): void {
    this.selectedDepartment = department;
    this.loadDepartmentEmployees(department.id);
    this.showManageEmployeesModal = true;
  }

  loadDepartmentEmployees(departmentId: number): void {
    this.departmentService.getDepartmentEmployees(departmentId).subscribe({
      next: (data) => {
        this.departmentEmployees = data;
      },
      error: (err) => console.error('Error loading department employees:', err)
    });
  }

  closeModals(): void {
    this.showCreateModal = false;
    this.showEditModal = false;
    this.showAssignModal = false;
    this.showManageEmployeesModal = false;
    this.selectedDepartment = null;
    this.departmentEmployees = [];
  }

  createDepartment(): void {
    this.departmentService.createDepartment(this.departmentForm).subscribe({
      next: () => {
        this.closeModals();
        this.loadDepartments();
      },
      error: (err) => console.error('Error creating department:', err)
    });
  }

  updateDepartment(): void {
    this.departmentService.updateDepartment(this.departmentForm.id, this.departmentForm).subscribe({
      next: () => {
        this.closeModals();
        this.loadDepartments();
      },
      error: (err) => console.error('Error updating department:', err)
    });
  }

  deleteDepartment(id: number): void {
    this.confirm.show('Are you sure you want to delete this department?', () => {
      this.departmentService.deleteDepartment(id).subscribe({
        next: () => {
          this.toast.show('Department deleted successfully', 'success');
          this.loadDepartments();
        },
        error: (err) => {
          this.toast.show('Failed to delete department', 'error');
          console.error('Error deleting department:', err);
        }
      });
    });
  }

  assignManager(departmentId: number, managerId: number): void {
    this.departmentService.assignManager(departmentId, managerId).subscribe({
      next: () => this.loadDepartments(),
      error: (err) => console.error('Error assigning manager:', err)
    });
  }

  assignEmployee(): void {
    if (this.selectedDepartment && this.assignForm.userId) {
      this.departmentService.assignEmployee(this.selectedDepartment.id, this.assignForm.userId).subscribe({
        next: () => {
          this.closeModals();
          this.loadDepartments();
          this.loadUnassignedUsers();
        },
        error: (err) => {
          console.error('Error assigning employee:', err);
          this.toast.show(err.error?.message || 'Failed to assign employee', 'error');
        }
      });
    }
  }

  removeEmployee(userId: number): void {
    this.confirm.show('Are you sure you want to remove this employee from the department?', () => {
      this.departmentService.removeEmployee(this.selectedDepartment.id, userId).subscribe({
        next: () => {
          this.toast.show('Employee removed successfully', 'success');
          this.loadDepartmentEmployees(this.selectedDepartment.id);
          this.loadDepartments();
          this.loadUnassignedUsers();
        },
        error: (err) => {
          this.toast.show('Failed to remove employee', 'error');
          console.error('Error removing employee:', err);
        }
      });
    });
  }

  changeDepartment(userId: number): void {
    const newDeptId = prompt('Enter new department ID:');
    if (newDeptId) {
      this.departmentService.changeEmployeeDepartment(userId, parseInt(newDeptId)).subscribe({
        next: () => {
          this.loadDepartmentEmployees(this.selectedDepartment.id);
          this.loadDepartments();
        },
        error: (err) => console.error('Error changing department:', err)
      });
    }
  }
}
