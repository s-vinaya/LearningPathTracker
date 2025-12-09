import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-weekly-hours',
  standalone: true,
  imports: [CommonModule, FormsModule, EmployeeSidebarComponent],
  templateUrl: './weekly-hours.html',
  styleUrls: ['./weekly-hours.css']
})
export class WeeklyHoursComponent implements OnInit {
  startTime = '';
  endTime = '';
  loading = true;
  saving = false;
  message = '';

  constructor(private employeeService: EmployeeService) {}

  ngOnInit() {
    this.loadSchedule();
  }

  loadSchedule() {
    this.employeeService.getLearningSchedule().subscribe({
      next: (data) => {
        this.startTime = data.startTime || '';
        this.endTime = data.endTime || '';
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading schedule:', err);
        this.loading = false;
      }
    });
  }

  saveSchedule() {
    if (!this.startTime || !this.endTime) {
      this.message = 'Please select both start and end times';
      return;
    }
    
    this.saving = true;
    this.employeeService.setLearningSchedule(this.startTime, this.endTime).subscribe({
      next: () => {
        this.message = 'Schedule saved! You will receive email reminders at your selected time.';
        this.saving = false;
      },
      error: (err) => {
        console.error('Error saving schedule:', err);
        this.message = 'Error saving schedule. Please try again.';
        this.saving = false;
      }
    });
  }
}
