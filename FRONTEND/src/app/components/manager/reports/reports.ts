import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ManagerService } from '../../../services/manager';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import jsPDF from 'jspdf';

@Component({
  selector: 'app-manager-reports',
  imports: [CommonModule, RouterModule, FormsModule, MatIconModule, ChartModule, TableModule, CardModule],
  templateUrl: './reports.component.html',
  styleUrls: ['../dashboard/manager-dashboard.css', './reports.css']
})
export class ManagerReportsComponent implements OnInit {
  selectedTimeFilter = 'Last 30 Days';
  detailedReports: any[] = [];
  summary: any = {};
  courseProgressData: any;
  progressOverTimeData: any;
  departmentStatsData: any;
  employeePointsData: any;
  loading = true;
  chartOptions: any;
  showExportModal = false;
  completionRate = 0;
  metrics = {
    activeLearners: 0,
    hoursCompleted: 0,
    pathsInProgress: 0,
    successRate: 0
  };

  constructor(private managerService: ManagerService) {
    this.chartOptions = {
      plugins: {
        legend: {
          labels: {
            color: '#495057'
          }
        }
      }
    };
  }

  ngOnInit() {
    this.loadReports();
  }

  loadReports() {
    this.loading = true;
    const managerId = JSON.parse(localStorage.getItem('user') || '{}').id;
    
    fetch(`https://localhost:7028/api/Reports/manager/${managerId}`)
      .then(res => res.json())
      .then((data: any) => {
        this.detailedReports = data.employees;
        this.summary = data.summary;
        
        this.completionRate = data.summary?.completionRate || 0;
        this.metrics = {
          activeLearners: data.summary?.activeLearners || 0,
          hoursCompleted: data.summary?.hoursCompleted || 0,
          pathsInProgress: data.summary?.pathsInProgress || 0,
          successRate: data.summary?.successRate || 0
        };
        
        this.courseProgressData = {
          labels: data.courseProgress.map((c: any) => this.trimCourseName(c.course)),
          datasets: [
            {
              label: 'Enrolled',
              data: data.courseProgress.map((c: any) => c.enrolled),
              backgroundColor: '#42A5F5'
            },
            {
              label: 'Completed',
              data: data.courseProgress.map((c: any) => c.completed),
              backgroundColor: '#66BB6A'
            }
          ]
        };

        this.progressOverTimeData = {
          labels: data.progressOverTime.map((p: any) => p.month),
          datasets: [
            {
              label: 'Completions',
              data: data.progressOverTime.map((p: any) => p.completions),
              fill: false,
              borderColor: '#42A5F5',
              tension: 0.4
            }
          ]
        };

        this.departmentStatsData = {
          labels: data.departmentStats.map((d: any) => d.department),
          datasets: [
            {
              data: data.departmentStats.map((d: any) => d.completions),
              backgroundColor: ['#42A5F5', '#66BB6A', '#FFA726', '#26C6DA', '#AB47BC']
            }
          ]
        };

        this.employeePointsData = {
          labels: data.employees.slice(0, 10).map((e: any) => e.name),
          datasets: [
            {
              label: 'Total Points',
              data: data.employees.slice(0, 10).map((e: any) => e.totalPoints),
              backgroundColor: '#66BB6A'
            }
          ]
        };

        this.loading = false;
      })
      .catch(err => {
        console.error('Error loading reports:', err);
        this.loading = false;
      });
  }

  getCompletionClass(rate: number): string {
    if (rate >= 75) return 'high';
    if (rate >= 50) return 'medium';
    return 'low';
  }

  trimCourseName(name: string, maxLength: number = 20): string {
    return name.length > maxLength ? name.substring(0, maxLength) + '...' : name;
  }

  openExportModal(): void {
    this.showExportModal = true;
  }

  closeExportModal(): void {
    this.showExportModal = false;
  }

  exportData(format: string): void {
    switch(format) {
      case 'pdf':
        this.exportToPDF();
        break;
      case 'excel':
        this.exportToExcel();
        break;
      case 'json':
        this.exportToJSON();
        break;
    }
    this.closeExportModal();
  }

  exportToPDF(): void {
    const doc = new jsPDF();
    
    doc.setFontSize(18);
    doc.text('Reports & Analytics', 14, 20);
    
    doc.setFontSize(12);
    doc.text('Summary Metrics', 14, 35);
    doc.setFontSize(10);
    doc.text(`Total Employees: ${this.summary.totalEmployees || 0}`, 14, 45);
    doc.text(`Total Completions: ${this.summary.totalCompletions || 0}`, 14, 52);
    doc.text(`Completion Rate: ${this.summary.avgCompletionRate || 0}%`, 14, 59);
    doc.text(`Total Points: ${this.summary.totalPoints || 0}`, 14, 66);
    
    doc.setFontSize(12);
    doc.text('Employee Performance Details', 14, 81);
    
    let yPos = 91;
    doc.setFontSize(8);
    doc.text('Name', 14, yPos);
    doc.text('Points', 70, yPos);
    doc.text('Completed', 95, yPos);
    doc.text('In Progress', 125, yPos);
    doc.text('Certificates', 160, yPos);
    doc.text('Avg %', 190, yPos);
    
    yPos += 7;
    this.detailedReports.forEach((emp) => {
      if (yPos > 270) {
        doc.addPage();
        yPos = 20;
      }
      doc.text(emp.name.substring(0, 25), 14, yPos);
      doc.text(emp.totalPoints.toString(), 70, yPos);
      doc.text(emp.completedCourses.toString(), 95, yPos);
      doc.text(emp.inProgressCourses.toString(), 125, yPos);
      doc.text(emp.certificates.toString(), 160, yPos);
      doc.text(`${emp.avgProgress.toFixed(0)}%`, 190, yPos);
      yPos += 7;
    });
    
    doc.save('reports.pdf');
  }

  exportToExcel(): void {
    const csv = [
      ['Name', 'Email', 'Department', 'Points', 'Completed', 'In Progress', 'Certificates', 'Avg Progress'],
      ...this.detailedReports.map(emp => [
        emp.name,
        emp.email,
        emp.department,
        emp.totalPoints,
        emp.completedCourses,
        emp.inProgressCourses,
        emp.certificates,
        `${emp.avgProgress.toFixed(2)}%`
      ])
    ].map(row => row.join(',')).join('\n');
    
    const blob = new Blob([csv], { type: 'text/csv' });
    this.downloadFile(blob, 'reports.csv');
  }

  exportToJSON(): void {
    const exportData = {
      summary: this.summary,
      detailedReports: this.detailedReports,
      courseProgress: this.courseProgressData,
      progressOverTime: this.progressOverTimeData,
      departmentStats: this.departmentStatsData,
      employeePoints: this.employeePointsData
    };
    const json = JSON.stringify(exportData, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    this.downloadFile(blob, 'reports.json');
  }

  downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}