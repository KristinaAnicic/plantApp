import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { LoginCredentials } from '../models/auth/login.interface';
import { Observable, tap } from 'rxjs';
import { LoginResponse } from '../models/auth/login-response.interface';
import { environment } from '../../environments/environment';
import { UserDto } from '../models/user.interface';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  
  constructor() {}

  currentUser = signal<UserDto | null>(this.getUser());
  isAuthenticated = signal<boolean>(!!localStorage.getItem('accessToken'));
  router = inject(Router);

  private getUser(): UserDto | null {
    const user = localStorage.getItem('user');
    try {
      return user ? JSON.parse(user) : null;
    } 
    catch {
      return null;
    }
  }

  isAdmin = computed(() => {
    const user = this.currentUser();
    return user?.role?.toLowerCase() === 'admin';
  });

  login(loginData: LoginCredentials): Observable<LoginResponse>{
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, loginData, { withCredentials: true })
      .pipe(
        tap((response) => {
          if (response && response.accessToken){
            this.updateLocalData(response);
          }
          else {
            console.error('Token not found');
            this.doLogoutCleanup();
          }
        })
      )
  }

  logout(): Observable<void>{
    this.doLogoutCleanup();
    return this.http.post<void>(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .pipe(
        tap({
          next: () => this.router.navigate(['/login']),
          error: (error) => {
            console.error('Error during logout', error);
            this.router.navigate(['/login']);
          }
        })
      )
  }

  refreshToken(): Observable<LoginResponse>{
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/refresh-token`, {},  { withCredentials: true })
      .pipe(
        tap((response) => {
          if (response && response.accessToken){
            this.updateLocalData(response);
          }
          else {
            console.error('Token not found in refresh response');
          }
        })
      )
  }

  private updateLocalData(response: LoginResponse) {
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    this.isAuthenticated.set(true);
    this.currentUser.set(response.user);
  }

  private doLogoutCleanup() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
  }
}
