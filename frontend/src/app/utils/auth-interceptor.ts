import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, filter, Observable, of, switchAll, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  // wating room for remaining requests
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.includes('/register') || req.url.includes('/login')) {
      return next.handle(req);
    }

    const token = localStorage.getItem("accessToken");

    if (token) {
      req = this.addToken(req, token);
    }
    
    return next.handle(req).pipe(
      catchError((error) => {
        if (token && error instanceof HttpErrorResponse && error.status === 401) {
          if (!this.isRefreshing) {
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);

            return this.authService.refreshToken().pipe(
              switchMap((res) => {               
                //push the new token into the subject to "release" waiting requests
                this.refreshTokenSubject.next(res.accessToken);
                this.isRefreshing = false;
                return next.handle(this.addToken(req, res.accessToken));
              }),
              catchError((err) => {
                this.isRefreshing = false;
                this.authService.logout().subscribe(); 
                return throwError(() => err);
              })
            );
          } 
          else {
            return this.refreshTokenSubject.pipe(
              filter((token) => token != null), //wait until the token is no longer null
              take(1),
              switchMap((jwt) => {
                return next.handle(this.addToken(req, jwt!));
              })
            )
          }               
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(req: HttpRequest<any>, token: string) {
    return req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
};
