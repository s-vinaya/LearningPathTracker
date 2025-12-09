import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ManagerService } from '../../../services/manager';

interface EmployeeHours {
  name: string;
  mon: string;
  tue: string;
  wed: string;
  thu: string;
  fri: string;
  total: string;
  status: string;
}

@Component({
  selector: 'app-weekly-hours',
  imports: [CommonModule, RouterModule, FormsModule, MatIconModule],
  templateUrl: './weekly-hours.component.html',
  styleUrls: ['../dashboard/manager-dashboard.css', './weekly-hours.css']
})
export class WeeklyHoursComponent implements OnInit {
  selectedTimeFilter = 'This Week';
  employeeHours: EmployeeHours[] = [];
  weeklyStats: any = {};
  loading = true;

  constructor(private managerService: ManagerService) {}

  ngOnInit() {
    this.loadWeeklyHours();
  }

  loadWeeklyHours() {
    this.loading = true;
    this.managerService.getWeeklyHours().subscribe({
      next: (data: any) => {
        this.employeeHours = (data.hours || []).map((hour: any) => ({
          name: hour.name,
          mon: hour.mon,
          tue: hour.tue,
          wed: hour.wed,
          thu: hour.thu,
          fri: hour.fri,
          total: hour.total,
          status: hour.status
        }));
        this.weeklyStats = data.stats || {};
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading weekly hours:', error);
        this.loading = false;
      }
    });
  }
}