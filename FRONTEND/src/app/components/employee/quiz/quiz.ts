import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-quiz',
  standalone: true,
  imports: [CommonModule, FormsModule, EmployeeSidebarComponent],
  templateUrl: './quiz.html',
  styleUrls: ['./quiz.css']
})
export class QuizComponent implements OnInit, OnDestroy {
  moduleId!: number;
  pathId!: number;
  quiz: any = null;
  answers: any = {};
  submitted = false;
  result: any = null;
  loading = true;
  timeLeft: number = 0;
  timerInterval: any;
  attemptsExhausted = false;
  resetTime: Date | null = null;
  showConfetti = false;
  isFinalQuiz = false;
  learningPathId: number | null = null;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private employeeService: EmployeeService,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.moduleId = +this.route.snapshot.params['id'];
    this.pathId = +this.route.snapshot.queryParams['pathId'];
    this.isFinalQuiz = this.route.snapshot.queryParams['type'] === 'final';
    this.learningPathId = this.route.snapshot.queryParams['learningPathId'] ? +this.route.snapshot.queryParams['learningPathId'] : null;
    this.checkAttemptsAndLoadQuiz();
    setInterval(() => {
      if (this.attemptsExhausted && this.resetTime) {
        const now = new Date();
        if (now >= this.resetTime) {
          this.attemptsExhausted = false;
          this.checkAttemptsAndLoadQuiz();
        }
      }
    }, 1000);
  }

  checkAttemptsAndLoadQuiz() {
    this.employeeService.getQuiz(this.moduleId).subscribe({
      next: (quizData) => {
        this.employeeService.checkQuizAttempts(quizData.id).subscribe({
          next: (attemptData) => {
            if (!attemptData.canAttempt) {
              this.attemptsExhausted = true;
              const utcTime = new Date(attemptData.resetTime);
              this.resetTime = new Date(utcTime.getTime() + (5.5 * 60 * 60 * 1000));
              this.loading = false;
            } else {
              this.quiz = quizData;
              this.timeLeft = quizData.timeLimit * 60;
              this.startTimer();
              this.loading = false;
            }
          },
          error: (err) => {
            console.error('Error checking attempts:', err);
            this.loading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error loading quiz:', err);
        this.loading = false;
      }
    });
  }

  loadQuiz() {
    this.checkAttemptsAndLoadQuiz();
  }

  startTimer() {
    this.timerInterval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.submitQuiz();
      }
    }, 1000);
  }

  get formattedTime(): string {
    const minutes = Math.floor(this.timeLeft / 60);
    const seconds = this.timeLeft % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  submitQuiz() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    const formattedAnswers: any = {};
    Object.keys(this.answers).forEach(key => {
      formattedAnswers[parseInt(key)] = this.answers[key];
    });
    this.employeeService.submitQuiz(this.quiz.id, { answers: formattedAnswers }).subscribe({
      next: (data) => {
        this.result = data;
        this.submitted = true;
        if (data.passed) {
          this.showConfetti = true;
          const redirectPath = this.isFinalQuiz && this.learningPathId 
            ? ['/employee/modules', this.learningPathId]
            : ['/employee/modules', this.pathId];
          setTimeout(() => {
            this.showConfetti = false;
            if (this.isFinalQuiz) {
              this.toast.show('🎉 Congratulations! You\'ve completed the learning path. Your certificate has been generated!', 'success');
            }
            this.router.navigate(redirectPath);
          }, 3000);
        } else if (data.remainingAttempts === 0) {
          this.resetTime = new Date();
          this.resetTime.setDate(this.resetTime.getDate() + 1);
          this.resetTime.setHours(0, 0, 0, 0);
        }
      },
      error: (err) => {
        console.error('Error submitting quiz:', err);
        const errorMsg = err.error?.message || 'Error submitting quiz';
        this.toast.show(errorMsg, 'error');
        if (errorMsg.includes('attempt limit')) {
          this.attemptsExhausted = true;
          this.resetTime = new Date();
          this.resetTime.setDate(this.resetTime.getDate() + 1);
          this.resetTime.setHours(0, 0, 0, 0);
        } else {
          this.router.navigate(['/employee/learning-paths']);
        }
      }
    });
  }

  retakeQuiz() {
    this.answers = {};
    this.submitted = false;
    this.result = null;
    this.loadQuiz();
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  get isQuizComplete(): boolean {
    return Object.keys(this.answers).length === this.quiz?.questions?.length;
  }

  get timeUntilReset(): string {
    if (!this.resetTime) return '';
    const now = new Date();
    const diff = this.resetTime.getTime() - now.getTime();
    if (diff <= 0) return 'Quiz available now';
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);
    return `${hours}h ${minutes}m ${seconds}s`;
  }
}
