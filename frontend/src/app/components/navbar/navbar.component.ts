import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LoginModal } from '../login-modal/login-modal';
import { Router, RouterLink } from "@angular/router";

@Component({
  selector: 'app-navbar',
  imports: [LoginModal, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class NavbarComponent {
  public authService = inject(AuthService);
  
  isUserMenuOpen = signal(false);
  isLoginOpen: boolean = false;
  isMobileMenuOpen = signal(false);

  toggleLoginModal() {
    this.isLoginOpen = !this.isLoginOpen;
  }

  toggleMenu() {
    this.isMobileMenuOpen.update(val => !val);
  }

  closeMenu() {
    this.isMobileMenuOpen.set(false);
  }

  signOut(){   
    this.authService.logout().subscribe({
      next: () => {
        this.isUserMenuOpen.set(false);
      },
      error: (err) => {
        console.error('Logout failed', err);
        this.isUserMenuOpen.set(false);
      }
    });  
  }
  
}
