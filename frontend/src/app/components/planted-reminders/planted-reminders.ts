import { Component, input, signal } from '@angular/core';
import { ReminderDto } from '../../models/reminder.interface';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-planted-reminders',
  imports: [DatePipe],
  templateUrl: './planted-reminders.html',
  styleUrl: './planted-reminders.css',
})
export class PlantedReminders {
  reminders = input<ReminderDto[] | undefined>();
  openedReminderMenuId = signal<number | null>(null);

  toggleReminderMenu(id: number, event: Event){
    event.stopPropagation();

    this.openedReminderMenuId.update(current => current === id ? null : id);
  }
}
