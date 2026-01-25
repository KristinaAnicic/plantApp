import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { ReminderDto, ReminderGetDto, ReminderReference, UpsertReminderDto } from '../models/reminder.interface';
import { environment } from '../../environments/environment';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class ReminderService {
  private http = inject(HttpClient)

  constructor() {}

  private references$ = this.getReferences().pipe(shareReplay(1));
  readonly references = toSignal<ReminderReference | null>(this.references$, { 
    initialValue: null 
  });

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
    return this.http.put(`${environment.apiUrl}/reminder/${id}/delay`, null, {params});
  }

  doneReminder(id: number, date?: Date) {
    var params = new HttpParams();
    if (date !== undefined){
      params = params.set('date', date.toISOString());
    }
    return this.http.put(`${environment.apiUrl}/reminder/${id}/done`, null, {params});
  }

  getReferences(): Observable<ReminderReference> {
      return this.http.get<ReminderReference>(`${environment.apiUrl}/reminder/references`);
    }
}
