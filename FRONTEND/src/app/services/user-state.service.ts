import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserStateService {
  private userImageSubject = new BehaviorSubject<string | null>(null);
  userImage$ = this.userImageSubject.asObservable();

  updateUserImage(imageBase64: string | null) {
    this.userImageSubject.next(imageBase64);
  }
}
