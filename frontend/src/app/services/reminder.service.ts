import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ReminderDto, ReminderGetDto, UpsertReminderDto } from '../models/reminder.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ReminderService {
  constructor(private http: HttpClient) {}

  getAllReminders(): Observable<ReminderDto[]> {
    return this.http.get<ReminderDto[]>(`${environment.apiUrl}/reminder`);
  }

  getReminder(id: number): Observable<ReminderGetDto> {
    return this.http.get<ReminderGetDto>(`${environment.apiUrl}/reminder/${id}`);
  }

  addReminder(reminder: UpsertReminderDto) {
    return this.http.post(`${environment.apiUrl}/reminder`, reminder);
  }

  updateReminder(id: number, reminder: UpsertReminderDto) {
    return this.http.put(`${environment.apiUrl}/reminder/${id}`, reminder);
  }

  removeReminder(id: number) {
    return this.http.delete(`${environment.apiUrl}/reminder/${id}`);
  }

  delayReminder(id: number, delayDays?: number) {
    var params = new HttpParams();
    if (delayDays !== undefined){
      params = params.set('delay', delayDays.toString());
    }
    return this.http.put(`${environment.apiUrl}/reminder/${id}/delay`, {params});
  }

  doneReminder(id: number, date?: Date) {
    var params = new HttpParams();
    if (date !== undefined){
      params = params.set('date', date.toISOString());
    }
    return this.http.put(`${environment.apiUrl}/reminder/${id}/done`, {params});
  }
}
