import { Component, inject, input, output, signal } from '@angular/core';
import { GrowthLogDto, GrowthLogGetDto } from '../../models/growth-log.interface';
import { DatePipe } from '@angular/common';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { GrowthLogService } from '../../services/growth-log.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-planted-growth-log',
  imports: [DatePipe],
  templateUrl: './planted-growth-log.html',
  styleUrl: './planted-growth-log.css',
})
export class PlantedGrowthLog {
  service = inject(GrowthLogService);
  notification = inject(NotificationService);
  logs = input<GrowthLogGetDto[] | undefined>();
  editLog = output<number>();
  logEdited = output<void>();

  openedLogMenuId = signal<number | null>(null);

  plantStatusTextColor(statusName?: string){
    const status = Object.values(PLANT_STATUS_MAP).find(s => s.name === statusName);
    if (!status)
      return 'text-gray-500';

    if (status.category === PlantStatusCategory.Healthy)
      return 'text-green-500'

    if (status.category === PlantStatusCategory.Stressed)
      return 'text-red-800'

    return 'text-gray-500';
  }

  toggleLogMenu(id: number, event: Event){
    event.stopPropagation();

    this.openedLogMenuId.update(current => current === id ? null : id);
  }

  editLogClick(id: number){
    this.editLog.emit(id);
    this.openedLogMenuId.set(null);
  }

  deleteLog(id: number){
    this.service.removeLog(id).subscribe({
      next: () => {
        this.notification.showSuccess("Successfully removed log")
        this.logEdited.emit();
      },
      error: () => this.notification.showError("Couldn't remove log, try again later!")
    });
    this.openedLogMenuId.set(null);
  }
}
