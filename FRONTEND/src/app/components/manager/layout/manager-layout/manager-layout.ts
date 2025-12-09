import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ManagerSidebarComponent } from '../../sidebar/manager-sidebar';
import { ManagerService } from '../../../../services/manager';

@Component({
  selector: 'app-manager-layout',
  imports: [CommonModule, RouterModule, ManagerSidebarComponent],
  templateUrl: './manager-layout.html',
  styleUrl: './manager-layout.css',
})
export class ManagerLayout implements OnInit {
  managerProfile: any = {};

  constructor(private managerService: ManagerService) {}

  ngOnInit(): void {
    this.managerService.getManagerProfile().subscribe({
      next: (profile) => {
        this.managerProfile = profile;
      },
      error: (error) => {
        console.error('Error loading manager profile:', error);
      }
    });
  }
}
