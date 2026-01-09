import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Login } from '../models/auth/login.interface';
import { Observable, tap, throwError } from 'rxjs';
import { LoginResponse } from '../models/auth/login-response.interface';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient, private router: Router) {}

  isUserLoggedIn(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  isUserAdmin(): boolean {
    const user = localStorage.getItem('user');
    if (!user) return false;
    
    const parsed = JSON.parse(user);
    const role = parsed?.role;
    if (!role) return false;

    return role.toLowerCase() == 'admin'; 
  }

  login(loginData: Login): Observable<LoginResponse>{
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, loginData, { withCredentials: true })
      .pipe(
        tap((response) => {
          if (response && response.accessToken){
            localStorage.setItem('accessToken', response.accessToken);
            localStorage.setItem('user', JSON.stringify(response.user));
          }
          else {
            console.error('Token not found');
          }
        })
      )
  }

  getUserId(): number | null {
    const user = localStorage.getItem('user');
    if (user) {
      return JSON.parse(user).id;
    }
    return null;
  }

  getName(): string {
    const user = localStorage.getItem('user');
    if (user) {
      return JSON.parse(user).name;
    }
    return '';
  }

  logout(): Observable<void>{
    return this.http.post<void>(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .pipe(
        tap({
          next: () => {
            this.doLogoutCleanup();
          },
          error: (error) => {
            console.error('Error during logout', error);
            this.doLogoutCleanup();
          }
        })
      )
  }

  refreshToken(): Observable<LoginResponse>{
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/refresh-token`, {},  { withCredentials: true })
      .pipe(
        tap((response) => {
          if (response && response.accessToken){
            localStorage.setItem('accessToken', response.accessToken);
            localStorage.setItem('user', JSON.stringify(response.user));
          }
          else {
            console.error('Token not found in refresh response');
          }
        })
      )
  }

  private doLogoutCleanup() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }
}
