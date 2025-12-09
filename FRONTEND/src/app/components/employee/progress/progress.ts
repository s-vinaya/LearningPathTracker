import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-progress',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './progress.html',
  styleUrls: ['./progress.css']
})
export class ProgressComponent implements OnInit {
  progress: any = {};
  loading = true;

  constructor(private employeeService: EmployeeService) {}

  ngOnInit() {
    this.loadProgress();
  }

  loadProgress() {
    this.employeeService.getProgress().subscribe({
      next: (data) => {
        this.progress = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading progress:', err);
        this.loading = false;
      }
    });
  }
}
