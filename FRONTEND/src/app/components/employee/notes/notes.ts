import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [CommonModule, FormsModule, EmployeeSidebarComponent],
  templateUrl: './notes.html',
  styleUrls: ['./notes.css']
})
export class NotesComponent implements OnInit {
  moduleId!: number;
  notes: any[] = [];
  newNote = '';
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService
  ) {}

  ngOnInit() {
    this.moduleId = +this.route.snapshot.params['id'];
    this.loadNotes();
  }

  loadNotes() {
    this.employeeService.getNotes(this.moduleId).subscribe({
      next: (data) => {
        this.notes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading notes:', err);
        this.loading = false;
      }
    });
  }

  saveNote() {
    if (!this.newNote.trim()) return;
    
    this.employeeService.saveNote(this.moduleId, { content: this.newNote }).subscribe({
      next: () => {
        this.newNote = '';
        this.loadNotes();
      },
      error: (err) => console.error('Error saving note:', err)
    });
  }

  deleteNote(noteId: number) {
    if (!confirm('Delete this note?')) return;
    
    this.employeeService.deleteNote(noteId).subscribe({
      next: () => this.loadNotes(),
      error: (err) => console.error('Error deleting note:', err)
    });
  }
}
