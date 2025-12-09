import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ManagerService } from '../../../services/manager';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

@Component({
  selector: 'app-manager-learning-paths',
  imports: [CommonModule, RouterModule, FormsModule, MatIconModule],
  templateUrl: './learning-paths.component.html',
  styleUrls: ['../dashboard/manager-dashboard.css', './learning-paths.css', './learning-paths-new.css']
})
export class ManagerLearningPathsComponent implements OnInit {
  teamMembers: any[] = [];
  selectedEmployees: number[] = [];
  selectAll: boolean = false;
  startDate = '';
  dueDate = '';
  managerNotes = '';
  dashboardStats: any = {};
  learningPaths: any[] = [];
  loading: boolean = true;
  error: string = '';

  constructor(private managerService: ManagerService, private toast: ToastService, private confirm: ConfirmService) {}

  ngOnInit(): void {
    this.loadLearningPaths();
    this.loadTeamMembers();
  }

  loadTeamMembers(): void {
    this.managerService.getTeamMembers().subscribe({
      next: (data: any) => {
        this.teamMembers = data.map((member: any) => ({
          id: member.id || member.Id,
          name: member.name || member.Name,
          assignedPaths: (member.assignedPaths || member.AssignedPaths || []).map((ap: any) => ({
            assignmentId: ap.assignmentId || ap.AssignmentId,
            learningPathId: ap.learningPathId || ap.LearningPathId,
            learningPathTitle: ap.learningPathTitle || ap.LearningPathTitle,
            dueDate: ap.dueDate || ap.DueDate,
            status: ap.status || ap.Status,
            progress: ap.progress || ap.Progress
          })),
          selected: false
        }));
      },
      error: (error) => {
        console.error('Error loading team members:', error);
      }
    });
  }

  loadLearningPaths(): void {
    this.loading = true;
    this.managerService.getLearningPaths().subscribe({
      next: (data: any) => {

        this.learningPaths = data.map((path: any) => {
          const courses = path.courses || path.Courses || path.modules || path.Modules || [];
          const mapped = {
            id: path.id || path.Id,
            title: path.title || path.Title,
            description: path.description || path.Description,
            duration: `${courses.length * 0.5}h`,
            level: path.level || path.Level || 'Beginner',
            courseCount: courses.length,
            courses: courses.map((c: any) => ({
              title: c.title || c.Title,
              description: c.description || c.Description,
              duration: '30 min'
            })),
            selected: false,
            disabled: false
          };
          console.log('Mapped path:', mapped);
          return mapped;
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading learning paths:', error);
        this.loading = false;
      }
    });
  }

  get selectedPathsCount(): number {
    return this.learningPaths.filter(path => path.selected).length;
  }

  get totalEstimatedHours(): number {
    const total = this.learningPaths
      .filter(path => path.selected)
      .reduce((sum, path) => {
        const duration = parseFloat(path.duration);
        return sum + (isNaN(duration) ? 0 : duration);
      }, 0);
    return total;
  }

  togglePathSelection(path: any): void {
    if (path.disabled) return;
    path.selected = !path.selected;
  }

  toggleEmployeeSelection(employee: any): void {
    employee.selected = !employee.selected;
    this.updateSelectedEmployees();
    this.updatePathAvailability();
  }

  toggleSelectAll(): void {
    this.selectAll = !this.selectAll;
    this.teamMembers.forEach(emp => emp.selected = this.selectAll);
    this.updateSelectedEmployees();
    this.updatePathAvailability();
  }

  updateSelectedEmployees(): void {
    this.selectedEmployees = this.teamMembers.filter(emp => emp.selected).map(emp => emp.id);
    this.selectAll = this.selectedEmployees.length === this.teamMembers.length;
  }

  updatePathAvailability(): void {
    const selectedMembers = this.teamMembers.filter(m => m.selected);
    
    if (selectedMembers.length === 0) {
      this.learningPaths.forEach(path => {
        path.disabled = false;
      });
      return;
    }
    
    this.learningPaths.forEach(path => {
      const anyHasPath = selectedMembers.some(member => 
        member.assignedPaths?.some((ap: any) => ap.learningPathId === path.id)
      );
      path.disabled = anyHasPath;
      if (anyHasPath) {
        path.selected = false;
      }
    });
  }

  assignLearningPaths(): void {
    const selectedPaths = this.learningPaths.filter(p => p.selected);
    
    if (selectedPaths.length === 0) {
      this.toast.show('Please select at least one learning path', 'info');
      return;
    }
    
    if (this.selectedEmployees.length === 0) {
      this.toast.show('Please select at least one employee', 'info');
      return;
    }

    // Check for duplicates
    const selectedMembers = this.teamMembers.filter(m => this.selectedEmployees.includes(m.id));
    const duplicates: string[] = [];

    selectedPaths.forEach(path => {
      selectedMembers.forEach(member => {
        const alreadyAssigned = member.assignedPaths?.some((ap: any) => ap.learningPathId === path.id);
        if (alreadyAssigned) {
          duplicates.push(`${member.name} already has ${path.title}`);
        }
      });
    });

    if (duplicates.length > 0) {
      this.toast.show('Cannot assign: ' + duplicates.join(', '), 'error');
      return;
    }

    let completed = 0;
    selectedPaths.forEach(path => {
      const assignment = {
        learningPathId: path.id,
        employeeIds: this.selectedEmployees,
        dueDate: this.dueDate || null,
        managerNotes: this.managerNotes
      };

      this.managerService.bulkAssignLearningPaths(assignment).subscribe({
        next: () => {
          completed++;
          if (completed === selectedPaths.length) {
            this.toast.show('Learning paths assigned successfully!', 'success');
            this.loadTeamMembers();
            this.resetForm();
          }
        },
        error: (error) => {
          console.error('Error assigning learning path:', error);
          this.toast.show(`Failed to assign ${path.title}`, 'error');
        }
      });
    });
  }

  bulkAssignToAll(): void {
    const selectedPaths = this.learningPaths.filter(p => p.selected);
    
    if (selectedPaths.length === 0) {
      this.toast.show('Please select at least one learning path', 'info');
      return;
    }

    this.confirm.show('Assign selected learning paths to ALL team members?', () => {
      const allEmployeeIds = this.teamMembers.map(m => m.id);

      let completed = 0;
      selectedPaths.forEach(path => {
        const assignment = {
          learningPathId: path.id,
          employeeIds: allEmployeeIds,
          dueDate: this.dueDate || null,
          managerNotes: this.managerNotes
        };

        this.managerService.bulkAssignLearningPaths(assignment).subscribe({
          next: () => {
            completed++;
            if (completed === selectedPaths.length) {
              this.toast.show('Learning paths assigned to all team members!', 'success');
              this.loadTeamMembers();
              this.resetForm();
            }
          },
          error: (error) => {
            console.error('Error bulk assigning:', error);
            this.toast.show(`Failed to assign ${path.title}`, 'error');
          }
        });
      });
    });
  }

  resetForm(): void {
    this.learningPaths.forEach(p => {
      p.selected = false;
      p.disabled = false;
    });
    this.teamMembers.forEach(e => e.selected = false);
    this.selectedEmployees = [];
    this.selectAll = false;
    this.dueDate = '';
    this.managerNotes = '';
  }

  unassignPath(event: Event, member: any, path: any): void {
    event.stopPropagation();
    
    this.confirm.show(`Remove ${path.learningPathTitle} from ${member.name}?`, () => {
      this.managerService.removeAssignment(path.assignmentId).subscribe({
        next: () => {
          this.toast.show('Learning path unassigned successfully!', 'success');
          this.loadTeamMembers();
          this.updatePathAvailability();
        },
        error: (error) => {
          console.error('Error unassigning learning path:', error);
          this.toast.show('Failed to unassign learning path', 'error');
        }
      });
    });
  }
}