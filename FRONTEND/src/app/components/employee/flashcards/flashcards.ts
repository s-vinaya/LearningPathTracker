import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-flashcards',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './flashcards.html',
  styleUrls: ['./flashcards.css']
})
export class FlashcardsComponent implements OnInit {
  moduleId!: number;
  flashcards: any[] = [];
  currentIndex = 0;
  flipped = false;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService
  ) {}

  ngOnInit() {
    this.moduleId = +this.route.snapshot.params['id'];
    this.loadFlashcards();
  }

  loadFlashcards() {
    this.employeeService.getFlashcards(this.moduleId).subscribe({
      next: (data) => {
        this.flashcards = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading flashcards:', err);
        this.loading = false;
      }
    });
  }

  get currentCard() {
    return this.flashcards[this.currentIndex];
  }

  flipCard() {
    this.flipped = !this.flipped;
  }

  nextCard() {
    if (this.currentIndex < this.flashcards.length - 1) {
      this.currentIndex++;
      this.flipped = false;
    }
  }

  prevCard() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.flipped = false;
    }
  }
}
