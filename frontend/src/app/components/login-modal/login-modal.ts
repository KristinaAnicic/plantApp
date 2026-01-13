import { Component, output } from '@angular/core';

@Component({
  selector: 'app-login-modal',
  imports: [],
  templateUrl: './login-modal.html',
  styleUrl: './login-modal.css',
})
export class LoginModal {
  close = output<void>();

  onCloseClick() {
    this.close.emit();
  }
}
