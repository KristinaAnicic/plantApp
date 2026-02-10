import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LoginModal } from '../login-modal/login-modal';
import { Router, RouterLink } from "@angular/router";
import { ReminderService } from '../../services/reminder.service';
import { ReminderDto } from '../../models/reminder.interface';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-navbar',
  imports: [LoginModal, RouterLink, DatePipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class NavbarComponent implements OnInit {
  public authService = inject(AuthService);
  public reminderService = inject(ReminderService);
  public router = inject(Router);
  
  isUserMenuOpen = signal(false);
  isLoginOpen: boolean = false;
  isMobileMenuOpen = signal(false);
  pendingReminders = signal<ReminderDto[] | null>(null);
  showReminders = signal(false);

  ngOnInit(): void {
    this.loadPendingReminders();
  }

  loadPendingReminders(){
    this.reminderService.getPendingReminders().subscribe((res) => {
      this.pendingReminders.set(res);
    })
  }

  navigateReminder(reminder: ReminderDto){
    this.showReminders.set(false);
    console.log(reminder);
    this.router.navigate(['/my-plants', reminder.plantedId]);
  }

  toggleShowReminders(){
    this.showReminders.update(val => !val);
  }

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
        this.router.navigate([''])
      },
      error: (err) => {
        console.error('Logout failed', err);
        this.isUserMenuOpen.set(false);
      }
    });  
  }
  redirectToHome() {
    this.router.navigate(['/']);
  }
  
}
