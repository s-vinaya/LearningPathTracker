import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-modules',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './modules.html',
  styleUrls: ['./modules.css']
})
export class ModulesComponent implements OnInit {
  pathId!: number;
  modules: any[] = [];
  loading = true;
  finalQuiz: any = null;
  showFinalQuiz = false;
  allCoursesCompleted = false;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private employeeService: EmployeeService
  ) {}

  ngOnInit() {
    this.pathId = +this.route.snapshot.params['id'];
    this.loadModules();
  }

  checkFinalQuiz() {
    this.employeeService.getLearningPathFinalQuiz(this.pathId).subscribe({
      next: (quiz) => {
        this.finalQuiz = quiz;
        this.showFinalQuiz = true;
      },
      error: () => {
        this.showFinalQuiz = false;
      }
    });
  }

  takeFinalQuiz() {
    if (this.finalQuiz) {
      this.router.navigate(['/employee/quiz', this.finalQuiz.id], { 
        queryParams: { type: 'final', learningPathId: this.pathId } 
      });
    }
  }

  loadModules() {
    this.employeeService.getAllAvailableLearningPaths().subscribe({
      next: (data: any) => {
        const learningPath = Array.isArray(data) ? data.find((lp: any) => lp.id === this.pathId) : null;
        if (learningPath?.courses) {
          this.employeeService.getEnrolledCourses().subscribe({
            next: (enrolledCourses: any) => {
              console.log('Enrolled courses data:', enrolledCourses);
              this.modules = learningPath.courses.map((course: any, index: number) => {
                const courseId = course.courseId || course.id;
                const enrollment = enrolledCourses.find((e: any) => e.id === courseId);
                const progress = enrollment?.progress || 0;
                const quizPassed = enrollment?.quizPassed || enrollment?.QuizPassed || false;
                const hasQuiz = course.hasQuiz || course.HasQuiz || enrollment?.hasQuiz || enrollment?.HasQuiz || false;
                console.log(`Course ${courseId}: hasQuiz=${hasQuiz}, quizPassed=${quizPassed}, progress=${progress}`, {course, enrollment});
                
                const prevCourse = index > 0 ? learningPath.courses[index - 1] : null;
                const prevCourseId = prevCourse ? (prevCourse.courseId || prevCourse.id) : null;
                const prevEnrollment = prevCourseId ? enrolledCourses.find((e: any) => e.id === prevCourseId) : null;
                const prevProgress = prevEnrollment?.progress || 0;
                const prevHasQuiz = prevEnrollment?.hasQuiz || false;
                const prevQuizPassed = prevEnrollment?.quizPassed || false;
                
                let isLocked = false;
                if (index > 0) {
                  if (prevHasQuiz) {
                    isLocked = !prevQuizPassed;
                  } else {
                    isLocked = prevProgress < 80;
                  }
                }
                
                return {
                  id: courseId,
                  name: course.title,
                  title: course.title,
                  thumbnail: course.thumbnail || course.thumbnailUrl || '',
                  locked: isLocked,
                  completed: progress >= 100 && (!hasQuiz || quizPassed),
                  canUnlock: false,
                  order: course.order,
                  progress: progress,
                  hasQuiz: hasQuiz,
                  quizPassed: quizPassed
                };
              });
              
              this.allCoursesCompleted = this.modules.every(m => m.completed);
              if (this.allCoursesCompleted) {
                this.checkFinalQuiz();
              }
              
              this.loading = false;
            },
            error: () => {
              this.modules = learningPath.courses.map((course: any, index: number) => ({
                id: course.id,
                name: course.title,
                title: course.title,
                thumbnail: course.thumbnail || course.thumbnailUrl || '',
                locked: index > 0,
                completed: false,
                canUnlock: false,
                order: course.order,
                progress: 0
              }));
              this.loading = false;
            }
          });
        } else {
          this.modules = [];
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Error loading modules:', err);
        this.modules = [];
        this.loading = false;
      }
    });
  }

  unlockModule(moduleId: number) {
    this.employeeService.unlockModule(moduleId).subscribe({
      next: () => {
        this.loadModules();
      },
      error: (err) => console.error('Error unlocking module:', err)
    });
  }

  startQuiz(courseId: number) {
    this.router.navigate(['/employee/course-viewer', courseId], { queryParams: { pathId: this.pathId } });
  }

  viewNotes(moduleId: number) {
    this.router.navigate(['/employee/notes', moduleId]);
  }

  viewFlashcards(moduleId: number) {
    this.router.navigate(['/employee/flashcards', moduleId]);
  }

  takeQuiz(courseId: number) {
    this.router.navigate(['/employee/quiz', courseId], { queryParams: { pathId: this.pathId } });
  }
}
