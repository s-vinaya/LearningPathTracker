import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface PlatformSettings {
  platformName: string;
  companyName: string;
  platformDescription: string;
  autoEnroll: boolean;
  certificates: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PlatformSettingsService {
  private apiUrl = 'https://localhost:7028/api/PlatformSettings';
  private settingsSubject = new BehaviorSubject<PlatformSettings>({
    platformName: 'LearnTrack',
    companyName: 'Your Company',
    platformDescription: 'A comprehensive learning management system for employee development',
    autoEnroll: true,
    certificates: true
  });

  public settings$ = this.settingsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadSettings();
  }

  private loadSettings(): void {
    this.http.get<PlatformSettings>(this.apiUrl).subscribe({
      next: (settings) => this.settingsSubject.next(settings),
      error: (err) => console.error('Error loading settings:', err)
    });
  }

  getSettings(): Observable<PlatformSettings> {
    return this.http.get<PlatformSettings>(this.apiUrl).pipe(
      tap(settings => this.settingsSubject.next(settings))
    );
  }

  updateSettings(settings: PlatformSettings): Observable<PlatformSettings> {
    return this.http.put<PlatformSettings>(this.apiUrl, settings).pipe(
      tap(updated => this.settingsSubject.next(updated))
    );
  }

  getCurrentSettings(): PlatformSettings {
    return this.settingsSubject.value;
  }
}
