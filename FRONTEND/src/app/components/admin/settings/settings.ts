import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar';
import { PlatformSettingsService } from '../../../services/platform-settings.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent],
  templateUrl: './settings.html',
  styleUrl: './settings.css',
})
export class Settings implements OnInit {
  platformName: string = '';
  companyName: string = '';
  platformDescription: string = '';
  autoEnroll: boolean = true;
  certificates: boolean = true;
  loading: boolean = false;

  constructor(private settingsService: PlatformSettingsService) {}

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.settingsService.getSettings().subscribe({
      next: (settings) => {
        this.platformName = settings.platformName;
        this.companyName = settings.companyName;
        this.platformDescription = settings.platformDescription;
        this.autoEnroll = settings.autoEnroll;
        this.certificates = settings.certificates;
      },
      error: (err) => console.error('Error loading settings:', err)
    });
  }

  saveSettings(): void {
    this.loading = true;
    const settings = {
      platformName: this.platformName,
      companyName: this.companyName,
      platformDescription: this.platformDescription,
      autoEnroll: this.autoEnroll,
      certificates: this.certificates
    };

    this.settingsService.updateSettings(settings).subscribe({
      next: () => {
        this.loading = false;
        alert('Settings saved successfully!');
      },
      error: (err) => {
        this.loading = false;
        console.error('Error saving settings:', err);
        alert('Failed to save settings');
      }
    });
  }

  cancel(): void {
    this.loadSettings();
  }
}
