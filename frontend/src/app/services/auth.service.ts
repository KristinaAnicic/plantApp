import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Login } from '../models/auth/login.interface';
import { Observable, tap, throwError } from 'rxjs';
import { LoginResponse } from '../models/auth/login-response.interface';
import { RefreshTokenRequest } from '../models/auth/refresh-token-request.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  router: any;
  constructor(private http: HttpClient) {}

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
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, loginData)
      .pipe(
        tap((response) => {
          if (response && response.accessToken && response.refreshToken){
            localStorage.setItem('accessToken', response.accessToken);
            localStorage.setItem('refreshToken', response.refreshToken);
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
    return this.http.post<void>(`${environment.apiUrl}/auth/logout`, {})
      .pipe(
        tap({
          next: () => {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('user');
            this.router.navigate('[/login]')
          },
          error: (error) => {
            console.error('Error during logout', error);
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('user');
            this.router.navigate('[/login]')
          }
        })
      )
  }

  refreshToken(): Observable<LoginResponse>{
    const refreshToken = localStorage.getItem('refreshToken');
    const user = localStorage.getItem('user');

    if (!refreshToken || !user) {
      const missing = !refreshToken ? 'Refresh token' : 'User';
      console.error(`${missing} not found`);
      return throwError(() => new Error(`${missing} not found`));
    }

    const request: RefreshTokenRequest = {
      userId: JSON.parse(user).id,
      refreshToken
    }

    return this.http.post<LoginResponse>(`${environment.apiUrl}/refresh-token`, request)
      .pipe(
        tap((response) => {
          if (response && response.accessToken && response.refreshToken){
            localStorage.setItem('accessToken', response.accessToken);
            localStorage.setItem('refreshToken', response.refreshToken);
            localStorage.setItem('user', JSON.stringify(response.user));
          }
          else {
            console.error('Token not found in refresh response');
          }
        })
      )
  }
}
