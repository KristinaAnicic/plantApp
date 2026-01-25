import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AddUserRatingDto, UpdateUserRatingDto, UserRatingDto } from '../models/user-rating.interface';

@Injectable({
  providedIn: 'root',
})
export class UserRatingService {
  private http = inject(HttpClient);
  
  constructor() {}

  getAllRatings(userId: number): Observable<UserRatingDto[]> {
    return this.http.get<UserRatingDto[]>(`${environment.apiUrl}/rating/${userId}`);
  }

  addRating(rating: AddUserRatingDto) {
    return this.http.post(`${environment.apiUrl}/rating`, rating);
  }

  updateRating(id: number, rating: UpdateUserRatingDto) {
    return this.http.put(`${environment.apiUrl}/rating/${id}`, rating);
  }

  removeRating(id: number) {
    return this.http.delete(`${environment.apiUrl}/rating/${id}`);
  }
}
