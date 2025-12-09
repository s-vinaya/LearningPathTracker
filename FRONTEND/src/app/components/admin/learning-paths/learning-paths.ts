import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { SidebarComponent } from '../sidebar/sidebar';
import { LearningPathService, LearningPath } from '../../../services/learning-path.service';
import { CourseService, Course } from '../../../services/course.service';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

@Component({
  selector: 'app-learning-paths',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, DragDropModule, MatIconModule],
  templateUrl: './learning-paths.html',
  styleUrl: './learning-paths.css',
})
export class LearningPaths implements OnInit {
  learningPaths: LearningPath[] = [];
  filteredPaths: LearningPath[] = [];
  searchTerm: string = '';
  selectedStatus: string = 'all';
  selectedLevel: string = 'all';
  loading: boolean = true;
  showCreateModal: boolean = false;
  showManageCoursesModal: boolean = false;
  statistics: any = { totalEnrollments: 0, totalCompletions: 0 };
  newPath = {
    title: '',
    description: '',
    level: 'Beginner',
    isActive: true
  };
  createAvailableCourses: Course[] = [];
  createSelectedCourses: Course[] = [];
  editingPath: LearningPath | null = null;
  selectedPath: LearningPath | null = null;
  allCourses: Course[] = [];
  availableCourses: Course[] = [];
  selectedCourses: Course[] = [];

  constructor(
    private learningPathService: LearningPathService,
    private courseService: CourseService,
    private toast: ToastService,
    private confirm: ConfirmService
  ) {}

  ngOnInit(): void {
    this.loadAllCourses();
    setTimeout(() => {
      this.loadLearningPaths();
      this.loadStatistics();
    }, 200);
  }

  loadAllCourses(): void {
    this.courseService.getAllCourses().subscribe({
      next: (courses) => {
        this.allCourses = courses;
      },
      error: (err) => console.error('Error loading courses:', err)
    });
  }

  loadLearningPaths(): void {
    this.loading = true;
    this.learningPathService.getAllLearningPaths().subscribe({
      next: (data) => {
        console.log('Raw API response:', data);
        this.learningPaths = data;
        console.log('Final learningPaths:', this.learningPaths);
        this.filteredPaths = this.learningPaths;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading learning paths:', err);
        this.loading = false;
      }
    });
  }

  get totalPaths(): number {
    return this.learningPaths.length;
  }

  get activePaths(): number {
    return this.learningPaths.filter(p => p.isActive).length;
  }

  get totalEnrollments(): number {
    return this.statistics.totalEnrollments || 0;
  }

  get totalCompletions(): number {
    return this.statistics.totalCompletions || 0;
  }

  loadStatistics(): void {
    this.learningPathService.getStatistics().subscribe({
      next: (data) => {
        this.statistics = data;
      },
      error: (err) => console.error('Error loading statistics:', err)
    });
  }

  filterPaths(): void {
    this.filteredPaths = this.learningPaths.filter(path => {
      const matchesSearch = path.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           path.description.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = this.selectedStatus === 'all' || 
                           (this.selectedStatus === 'active' && path.isActive) ||
                           (this.selectedStatus === 'inactive' && !path.isActive);
      const matchesLevel = this.selectedLevel === 'all' || path.level === this.selectedLevel;
      return matchesSearch && matchesStatus && matchesLevel;
    });
  }

  deletePath(id: number): void {
    this.confirm.show('Are you sure you want to delete this learning path?', () => {
      this.learningPathService.deleteLearningPath(id).subscribe({
        next: () => {
          this.toast.show('Learning path deleted successfully', 'success');
          this.loadLearningPaths();
        },
        error: (err) => {
          this.toast.show('Failed to delete learning path', 'error');
          console.error('Error deleting path:', err);
        }
      });
    });
  }

  openCreateModal(): void {
    this.editingPath = null;
    this.newPath = { title: '', description: '', level: 'Beginner', isActive: true };
    this.createSelectedCourses = [];
    this.createAvailableCourses = [...this.allCourses];
    this.showCreateModal = true;
  }

  editPath(path: LearningPath): void {
    this.editingPath = path;
    this.newPath = {
      title: path.title,
      description: path.description,
      level: path.level,
      isActive: path.isActive
    };
    this.createSelectedCourses = (path.courses || []).map(m => 
      this.allCourses.find(c => c.title === m.title)
    ).filter(c => c !== undefined) as Course[];
    this.updateCreateAvailableCourses();
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createPath(): void {
    if (!this.newPath.title) {
      this.toast.show('Please enter a title', 'error');
      return;
    }

    const modules = this.createSelectedCourses.map((course, index) => ({
      title: course.title,
      description: course.description,
      order: index + 1
    }));

    if (this.editingPath) {
      const pathData = { ...this.newPath };
      this.learningPathService.updateLearningPath(this.editingPath.id, pathData).subscribe({
        next: () => {
          this.learningPathService.updateLearningPathModules(this.editingPath!.id, modules).subscribe({
            next: () => {
              this.closeCreateModal();
              this.loadLearningPaths();
            },
            error: (err) => {
              console.error('Error updating modules:', err);
              this.toast.show('Failed to update modules', 'error');
            }
          });
        },
        error: (err) => {
          console.error('Error updating learning path:', err);
          this.toast.show('Failed to update learning path', 'error');
        }
      });
    } else {
      const pathData = { ...this.newPath, modules };
      this.learningPathService.createLearningPath(pathData).subscribe({
        next: () => {
          this.closeCreateModal();
          this.loadLearningPaths();
        },
        error: (err) => {
          console.error('Error creating learning path:', err);
          this.toast.show('Failed to create learning path', 'error');
        }
      });
    }
  }

  updateCreateAvailableCourses(): void {
    const selectedIds = this.createSelectedCourses.map(c => c.id);
    this.createAvailableCourses = this.allCourses.filter(c => !selectedIds.includes(c.id));
  }

  addCourseToNew(course: Course): void {
    this.createSelectedCourses.push(course);
    this.updateCreateAvailableCourses();
  }

  removeCourseFromNew(index: number): void {
    this.createSelectedCourses.splice(index, 1);
    this.updateCreateAvailableCourses();
  }

  drop(event: CdkDragDrop<Course[]>): void {
    moveItemInArray(this.createSelectedCourses, event.previousIndex, event.currentIndex);
  }

  openManageCoursesModal(path: LearningPath): void {
    this.selectedPath = path;
    this.loadAllCourses();
    setTimeout(() => {
      this.selectedCourses = (path.courses || []).map(m => 
        this.allCourses.find(c => c.title === m.title)
      ).filter(c => c !== undefined) as Course[];
      this.updateAvailableCourses();
    }, 100);
    this.showManageCoursesModal = true;
  }

  closeManageCoursesModal(): void {
    this.showManageCoursesModal = false;
    this.selectedPath = null;
    this.selectedCourses = [];
    this.availableCourses = [];
  }

  updateAvailableCourses(): void {
    const selectedIds = this.selectedCourses.map(c => c.id);
    this.availableCourses = this.allCourses.filter(c => !selectedIds.includes(c.id));
  }

  addCourse(course: Course): void {
    this.selectedCourses.push(course);
    this.updateAvailableCourses();
  }

  removeCourse(index: number): void {
    this.selectedCourses.splice(index, 1);
    this.updateAvailableCourses();
  }

  moveUp(index: number): void {
    if (index > 0) {
      [this.selectedCourses[index], this.selectedCourses[index - 1]] = 
      [this.selectedCourses[index - 1], this.selectedCourses[index]];
    }
  }

  moveDown(index: number): void {
    if (index < this.selectedCourses.length - 1) {
      [this.selectedCourses[index], this.selectedCourses[index + 1]] = 
      [this.selectedCourses[index + 1], this.selectedCourses[index]];
    }
  }

  savePathCourses(): void {
    if (!this.selectedPath) return;

    const modules = this.selectedCourses.map((course, index) => ({
      title: course.title,
      description: course.description,
      order: index + 1,
      learningPathId: this.selectedPath!.id
    }));

    this.learningPathService.updateLearningPathModules(this.selectedPath.id, modules).subscribe({
      next: () => {
        this.closeManageCoursesModal();
        this.loadLearningPaths();
      },
      error: (err) => {
        console.error('Error updating path courses:', err);
        this.toast.show('Failed to update courses', 'error');
      }
    });
  }
}
