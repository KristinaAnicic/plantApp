import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LoginModal } from '../login-modal/login-modal';

@Component({
  selector: 'app-navbar',
  imports: [LoginModal],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class NavbarComponent implements OnInit {
  private authService = inject(AuthService);

  isLoggedIn: boolean = false;
  isLoginOpen: boolean = false;

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isUserLoggedIn();
  }

  toggleLoginModal() {
    this.isLoginOpen = !this.isLoginOpen;
  }
  
}
