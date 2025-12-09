import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { SidebarComponent } from '../sidebar/sidebar';
import { QuizService, Quiz, QuizQuestion } from '../../../services/quiz.service';
import { CourseService, Course } from '../../../services/course.service';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';


@Component({
  selector: 'app-manage-quizzes',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, DragDropModule, MatIconModule],
  templateUrl: './manage-quizzes.html',
  styleUrl: './manage-quizzes.css'
})
export class ManageQuizzesComponent implements OnInit {
  quizzes: Quiz[] = [];
  filteredQuizzes: Quiz[] = [];
  courses: Course[] = [];

  searchTerm: string = '';
  selectedType: string = 'all';
  loading: boolean = true;
  showModal: boolean = false;
  editingQuiz: Quiz | null = null;
  
  newQuiz = {
    title: '',
    description: '',
    courseId: null as number | null,
    learningPathId: null as number | null,
    isFinalQuiz: false,
    passingScore: 70,
    timeLimit: 30,
    isActive: true,
    questions: [] as any[]
  };

  learningPaths: any[] = [];
  quizType: string = 'course';

  editingQuestionIndex: number | null = null;

  constructor(
    private quizService: QuizService,
    private courseService: CourseService,
    private toast: ToastService,
    private confirm: ConfirmService
  ) {}

  ngOnInit(): void {
    this.loadQuizzes();
    this.loadCourses();
    this.loadLearningPaths();
  }

  loadQuizzes(): void {
    this.loading = true;
    this.quizService.getAllQuizzes().subscribe({
      next: (data) => {
        this.quizzes = data;
        this.filteredQuizzes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading quizzes:', err);
        this.loading = false;
      }
    });
  }

  loadCourses(): void {
    this.courseService.getAllCourses().subscribe({
      next: (data) => this.courses = data,
      error: (err) => console.error('Error loading courses:', err)
    });
  }

  loadLearningPaths(): void {
    this.courseService.getAllLearningPaths().subscribe({
      next: (data) => {
        this.learningPaths = data;
        console.log('Learning paths loaded:', this.learningPaths);
      },
      error: (err) => console.error('Error loading learning paths:', err)
    });
  }



  get totalQuizzes(): number {
    return this.quizzes.length;
  }

  get courseQuizzes(): number {
    return this.quizzes.filter(q => q.courseId).length;
  }

  get learningPathQuizzes(): number {
    return this.quizzes.filter(q => q.learningPathId && !q.isFinalQuiz).length;
  }

  get finalQuizzes(): number {
    return this.quizzes.filter(q => q.isFinalQuiz).length;
  }

  filterQuizzes(): void {
    this.filteredQuizzes = this.quizzes.filter(quiz => {
      const matchesSearch = quiz.title.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesType = this.selectedType === 'all' ||
        (this.selectedType === 'course' && quiz.courseId) ||
        (this.selectedType === 'learningpath' && quiz.learningPathId && !quiz.isFinalQuiz) ||
        (this.selectedType === 'final' && quiz.isFinalQuiz);
      return matchesSearch && matchesType;
    });
  }

  openModal(): void {
    this.editingQuiz = null;
    this.editingQuestionIndex = null;
    this.quizType = 'course';
    this.newQuiz = {
      title: '',
      description: '',
      courseId: null,
      learningPathId: null,
      isFinalQuiz: false,
      passingScore: 70,
      timeLimit: 30,
      isActive: true,
      questions: [{ ...this.createEmptyQuestion(), isEditing: true }]
    };
    this.editingQuestionIndex = 0;
    this.showModal = true;
  }

  editQuiz(quiz: Quiz): void {
    this.editingQuiz = quiz;
    this.editingQuestionIndex = null;
    this.quizType = quiz.courseId ? 'course' : 'learningpath';
    this.newQuiz = {
      title: quiz.title,
      description: quiz.description,
      courseId: quiz.courseId || null,
      learningPathId: quiz.learningPathId || null,
      isFinalQuiz: quiz.isFinalQuiz || false,
      passingScore: quiz.passingScore,
      timeLimit: quiz.timeLimit,
      isActive: quiz.isActive,
      questions: quiz.questions.map(q => ({
        question: q.question,
        optionA: q.optionA,
        optionB: q.optionB,
        optionC: q.optionC,
        optionD: q.optionD,
        correctAnswer: q.correctAnswer,
        points: q.points,
        isEditing: false
      }))
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  createEmptyQuestion(): any {
    return {
      question: '',
      optionA: '',
      optionB: '',
      optionC: '',
      optionD: '',
      correctAnswer: 'A',
      points: 10
    };
  }

  addQuestion(): void {
    this.newQuiz.questions.unshift({ ...this.createEmptyQuestion(), isEditing: true });
    this.editingQuestionIndex = 0;
  }

  dropQuestion(event: CdkDragDrop<any[]>): void {
    moveItemInArray(this.newQuiz.questions, event.previousIndex, event.currentIndex);
    if (this.editingQuestionIndex !== null) {
      if (event.previousIndex === this.editingQuestionIndex) {
        this.editingQuestionIndex = event.currentIndex;
      }
    }
  }

  editQuestion(index: number): void {
    this.newQuiz.questions.forEach((q: any) => q.isEditing = false);
    this.newQuiz.questions[index].isEditing = true;
    this.editingQuestionIndex = index;
  }

  saveQuestion(index: number): void {
    this.newQuiz.questions[index].isEditing = false;
    this.editingQuestionIndex = null;
  }

  removeQuestion(index: number): void {
    this.newQuiz.questions.splice(index, 1);
  }

  onQuizTypeChange(): void {
    this.newQuiz.courseId = null;
    this.newQuiz.learningPathId = null;
    this.newQuiz.isFinalQuiz = this.quizType === 'learningpath';
  }

  saveQuiz(): void {
    if (!this.newQuiz.title || this.newQuiz.questions.length === 0) {
      this.toast.show('Please enter title and at least one question', 'error');
      return;
    }

    if (this.quizType === 'course' && !this.newQuiz.courseId) {
      this.toast.show('Please select a course', 'error');
      return;
    }

    if (this.quizType === 'learningpath' && !this.newQuiz.learningPathId) {
      this.toast.show('Please select a learning path', 'error');
      return;
    }

    if (this.quizType === 'course') {
      this.newQuiz.learningPathId = null;
      this.newQuiz.isFinalQuiz = false;
    } else {
      this.newQuiz.courseId = null;
      this.newQuiz.isFinalQuiz = true;
    }

    if (this.editingQuiz) {
      this.quizService.updateQuiz(this.editingQuiz.id, this.newQuiz).subscribe({
        next: () => {
          this.closeModal();
          this.loadQuizzes();
        },
        error: (err) => console.error('Error updating quiz:', err)
      });
    } else {
      this.quizService.createQuiz(this.newQuiz).subscribe({
        next: () => {
          this.closeModal();
          this.loadQuizzes();
        },
        error: (err) => console.error('Error creating quiz:', err)
      });
    }
  }

  deleteQuiz(id: number): void {
    this.confirm.show('Are you sure you want to delete this quiz?', () => {
      this.quizService.deleteQuiz(id).subscribe({
        next: () => {
          this.toast.show('Quiz deleted successfully', 'success');
          this.loadQuizzes();
        },
        error: (err) => {
          this.toast.show('Failed to delete quiz', 'error');
          console.error('Error deleting quiz:', err);
        }
      });
    });
  }
}
