import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { AddUserDto, UpdateUserDto, UserDto, UserGetDto } from '../models/user.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  
  constructor() {}

  getAllUsers(): Observable<UserDto[]>{
    return this.http.get<UserDto[]>(`${environment.apiUrl}/user`);
  }

  getUserData(id: number): Observable<UserGetDto>{
    return this.http.get<UserGetDto>(`${environment.apiUrl}/user/${id}`);
  }

  addUser(user: AddUserDto) {
    return this.http.post(`${environment.apiUrl}/user`, user);
  }

  updateUser(id: number, user: UpdateUserDto) {
    return this.http.put(`${environment.apiUrl}/user/${id}`, user);
  }

  deleteUser(id: number) {
    return this.http.delete(`${environment.apiUrl}/user/${id}`);
  }
}
