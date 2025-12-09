import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-badges',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './badges.html',
  styleUrls: ['./badges.css']
})
export class BadgesComponent implements OnInit {
  badges: any[] = [];
  achievements: any[] = [];
  streaks: any = {};
  loading = true;

  constructor(private employeeService: EmployeeService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.employeeService.getBadges().subscribe({
      next: (data) => this.badges = data,
      error: (err) => console.error('Error loading badges:', err)
    });

    this.employeeService.getAchievements().subscribe({
      next: (data) => this.achievements = data,
      error: (err) => console.error('Error loading achievements:', err)
    });

    this.employeeService.getStreaks().subscribe({
      next: (data) => {
        this.streaks = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading streaks:', err);
        this.loading = false;
      }
    });
  }
}
