import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  show(message: string, onConfirm: () => void) {
    const overlay = document.createElement('div');
    overlay.className = 'confirm-overlay';
    
    const modal = document.createElement('div');
    modal.className = 'confirm-modal';
    modal.innerHTML = `
      <div class="confirm-content">
        <p>${message}</p>
        <div class="confirm-buttons">
          <button class="confirm-btn cancel">Cancel</button>
          <button class="confirm-btn confirm">Confirm</button>
        </div>
      </div>
    `;
    
    overlay.appendChild(modal);
    document.body.appendChild(overlay);
    
    setTimeout(() => overlay.classList.add('show'), 10);
    
    const close = () => {
      overlay.classList.remove('show');
      setTimeout(() => document.body.removeChild(overlay), 300);
    };
    
    modal.querySelector('.cancel')?.addEventListener('click', close);
    modal.querySelector('.confirm')?.addEventListener('click', () => {
      close();
      onConfirm();
    });
  }
}
