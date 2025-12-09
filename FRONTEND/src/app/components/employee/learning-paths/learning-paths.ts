import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-employee-learning-paths',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './learning-paths.html',
  styleUrls: ['./learning-paths.css']
})
export class EmployeeLearningPathsComponent implements OnInit {
  learningPaths: any[] = [];
  availablePaths: any[] = [];
  allCourses: any[] = [];
  loading = true;
  showAvailable = false;

  constructor(
    private employeeService: EmployeeService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadCourses();
    this.loadLearningPaths();
  }

  loadCourses() {
    this.employeeService.getAllAvailableCourses().subscribe({
      next: (courses) => this.allCourses = courses,
      error: (err) => console.error('Error loading courses:', err)
    });
  }

  loadLearningPaths() {
    this.employeeService.getAssignedLearningPaths().subscribe({
      next: (assignedPaths) => {
        this.employeeService.getAllAvailableLearningPaths().subscribe({
          next: (allPaths) => {
            this.employeeService.getEnrolledCourses().subscribe({
              next: (enrolledCourses: any[]) => {
                this.learningPaths = assignedPaths.map((assigned: any) => {
                  const fullPath = allPaths.find((p: any) => p.id === assigned.id);
                  const courses = fullPath?.courses || [];
                  const avgProgress = courses.length > 0 
                    ? Math.round(courses.reduce((sum: number, course: any) => {
                        const courseId = course.courseId || course.id;
                        const enrollment = enrolledCourses.find((e: any) => e.id === courseId);
                        return sum + (enrollment?.progress || 0);
                      }, 0) / courses.length)
                    : 0;
                  
                  let status = 'Not Started';
                  if (assigned.certificateGenerated) {
                    status = 'Completed';
                  } else if (avgProgress > 0) {
                    status = 'In Progress';
                  }
                  
                  return { ...assigned, courses, progress: avgProgress, status };
                });
                this.loading = false;
                this.loadAvailablePaths();
              },
              error: () => {
                this.learningPaths = assignedPaths.map((assigned: any) => {
                  const fullPath = allPaths.find((p: any) => p.id === assigned.id);
                  const status = assigned.certificateGenerated ? 'Completed' : 'Not Started';
                  return { ...assigned, courses: fullPath?.courses || [], progress: 0, status };
                });
                this.loading = false;
                this.loadAvailablePaths();
              }
            });
          },
          error: () => {
            this.learningPaths = assignedPaths;
            this.loading = false;
            this.loadAvailablePaths();
          }
        });
      },
      error: (err) => {
        console.error('Error loading learning paths:', err);
        this.loading = false;
      }
    });
  }

  loadAvailablePaths() {
    this.employeeService.getAllAvailableLearningPaths().subscribe({
      next: (data) => {
        const enrolledIds = this.learningPaths.map(p => p.id);
        this.availablePaths = data.filter((path: any) => !enrolledIds.includes(path.id));
      },
      error: (err) => console.error('Error loading available paths:', err)
    });
  }

  enrollInPath(pathId: number) {
    this.employeeService.enrollInLearningPath(pathId).subscribe({
      next: (response) => {
        alert('Successfully enrolled in the learning path!');
        this.loadLearningPaths();
        this.loadAvailablePaths();
      },
      error: (err) => {
        const message = err.error?.message || 'Failed to enroll in learning path';
        alert(message);
        console.error('Error enrolling:', err);
      }
    });
  }

  viewModules(pathId: number) {
    this.router.navigate(['/employee/modules', pathId]);
  }
}
