import { Component, computed, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { ReminderService } from '../../services/reminder.service';
import { NotificationService } from '../../services/notification.service';
import { UpsertReminderDto } from '../../models/reminder.interface';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NumbersOnlyDirective } from '../../directives/numbers-only.directive';
import { DateUtils } from '../../utils/date-utils';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-add-edit-reminder-modal',
  imports: [ReactiveFormsModule, NumbersOnlyDirective, TranslateModule],
  templateUrl: './add-edit-reminder-modal.html',
  styleUrl: './add-edit-reminder-modal.css',
})
export class AddEditReminderModal implements OnInit{
  private service = inject(ReminderService);
  private notificationService = inject(NotificationService);
  private translate = inject(TranslateService);

  editReminder = input<UpsertReminderDto | null>(null);
  references = this.service.references;
  plantedId = input<number | null>(null);
  reminderEdited = output<void>();
  close = output<void>();
  showWarning = signal(false);
  errorMessage = signal('');
  minDate = DateUtils.formatDateForInput(new Date().toISOString());

  isEditing = computed(() => !!this.editReminder());
  headerText = computed(() => this.isEditing() ? this.translate.instant('reminderForm.editReminderTitle') : this.translate.instant('reminderForm.addReminderTitle'));
  buttonText = computed(() => this.isEditing() ? this.translate.instant('forms.saveChanges') : this.translate.instant('reminder.setReminder'));

  reminderForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    plantedId: new FormControl<number | undefined>(0, { nonNullable: true }),
    reminderTypeId: new FormControl<number | undefined>(0, { nonNullable: true }),
    frequencyTypeId: new FormControl<number | undefined>(0, { nonNullable: true }),
    frequencyNum: new FormControl<number>(1, { nonNullable: true }),
    originalDueDate: new FormControl(DateUtils.formatDateForInput(new Date().toISOString()), { nonNullable: true }),
    note: new FormControl('', { nonNullable: true })
  })

  ngOnInit(): void {
    const reminder = this.editReminder();
    const refs = this.references();

    if (refs && refs.reminderTypes.length > 0 && refs.frequencyTypes.length > 0){
      if (reminder) {
        this.reminderForm.patchValue({ 
            ...reminder,
            originalDueDate: DateUtils.formatDateForInput(reminder.originalDueDate)
          }, { emitEvent: false });
      }
      else {
        this.reminderForm.patchValue({ 
          reminderTypeId: refs.reminderTypes[0].id,
          frequencyTypeId: refs.frequencyTypes[0].id,
          plantedId: this.plantedId() ?? 0,
          originalDueDate: DateUtils.formatDateForInput(new Date().toISOString())
        }, { emitEvent: false });
      }
    }
  }

  addEditReminder(){
    if (this.reminderForm.invalid) return;
      const data = this.reminderForm.getRawValue();   

    if (data.originalDueDate) {
      const localDate = new Date(data.originalDueDate);
      const utcDate = localDate.toISOString(); 
      data.originalDueDate = utcDate;
    }

    const cleanData: UpsertReminderDto = {
      ...data, 
      plantedId: Number(data.plantedId),
      frequencyTypeId: Number(data.frequencyTypeId),
      reminderTypeId: Number(data.reminderTypeId),
      note: data.note?.trim() || undefined
    };
  
    if (this.isEditing()){
      const id = this.editReminder()?.id;

      console.log(id);
      if (id === undefined) {
        console.error("Cannot save changes for reminder without providing id.");
        return;
      }
      
      this.service.updateReminder(id, cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully saved changes');   
          this.reminderEdited.emit();
          this.close.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on editing reminder: ", err);
        }
      })
    }
    
    else {
      this.service.addReminder(cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully set reminder');
          this.close.emit();
          this.reminderEdited.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on setting reminder: ", err);
        }
      })
    }
  }

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }
}
