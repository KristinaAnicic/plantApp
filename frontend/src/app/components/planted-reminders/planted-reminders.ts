import { Component, computed, inject, input, output, signal } from '@angular/core';
import { ReminderGetDto } from '../../models/reminder.interface';
import { DatePipe } from '@angular/common';
import { ReminderService } from '../../services/reminder.service';
import { NotificationService } from '../../services/notification.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { NumbersOnlyDirective } from '../../directives/numbers-only.directive';
import { map } from 'rxjs';
import { Router, RouterLink } from "@angular/router";
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-planted-reminders',
  imports: [DatePipe, ReactiveFormsModule, NumbersOnlyDirective, TranslateModule],
  templateUrl: './planted-reminders.html',
  styleUrl: './planted-reminders.css',
})
export class PlantedReminders {
  service = inject(ReminderService);
  notif = inject(NotificationService);
  router = inject(Router);
  
  reminders = input<ReminderGetDto[] | undefined>();
  isGroup = input<boolean | null>(null);
  editReminder = output<number>();
  reminderEdited = output<void>();

  openedReminderMenuId = signal<number | null>(null);
  isDelayModalOpen = signal(false);
  previewDate: Date = new Date();

  toggleReminderMenu(id: number, event: Event){
    event.stopPropagation();

    this.openedReminderMenuId.update(current => current === id ? null : id);
  }

  editReminderClick(id: number){
    this.editReminder.emit(id);
    this.openedReminderMenuId.set(null);
  }

  markAsDone(id: number){
    this.service.doneReminder(id).subscribe({
      next: () => {
        this.notif.showSuccess("Marked as done")
        this.reminderEdited.emit();
      },
      error: () => this.notif.showError("Couldn't mark as done, try again later!")
    });
    this.openedReminderMenuId.set(null);
  }

  deleteReminder(id: number){
    this.service.removeReminder(id).subscribe({
      next: () => {
        this.notif.showSuccess("Successfully removed log")
        this.reminderEdited.emit();
      },
      error: () => this.notif.showError("Couldn't remove log, try again later!")
    });
    this.openedReminderMenuId.set(null);
  }

  toggleDelayModal(){
    this.isDelayModalOpen.update(val => !val); 
  }

  delayReminder(){
    if (this.delayForm.invalid) return;

    const id = this.openedReminderMenuId();
    if (!id) return;

    const delayValue = this.delayForm.get('delay')?.value;
    const delay: number = Number(delayValue);
    this.service.delayReminder(id, delay).subscribe({
      next: () => {
        this.notif.showSuccess("Reminder is delayed")
        this.reminderEdited.emit();
      },
      error: () => this.notif.showError("Couldn't delay reminder, try again later!")
    });
    this.openedReminderMenuId.set(null);
    this.isDelayModalOpen.set(false);
  }

  isDue(reminder: any): boolean {
    const today = new Date();
    today.setHours(0,0,0,0);
    const dueDate = new Date(reminder.nextDueDate);
    dueDate.setHours(0, 0, 0, 0);
    return dueDate <= today; 
  }

  delayForm = new FormGroup({
    delay: new FormControl<string>("1", {validators: [Validators.required, Validators.min(1)], nonNullable: true })
  })

  delayValue = toSignal(
    this.delayForm.get('delay')!.valueChanges.pipe(
    map(value => Number(value) || 0)
    ), 
    { initialValue: 1 }
  );

  currentPreview = computed(() => {
    const days = this.delayValue();
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    today.setDate(today.getDate() + days);
    return today;
  });

  readonly days = [
    { label: '+1 day', value: 1 },
    { label: '+2 days', value: 2},
    { label: '+3 days', value: 3},
    { label: '+1 week', value: 7}
  ];

  setNewDelayValue(del: number){
    this.delayForm.patchValue({delay: del.toString()});
    this.delayForm.get('delay')?.markAsDirty();
  }

  navigateToPlanted(plantedId : number){
    this.router.navigate(['my-plants', plantedId]);
  }
}
