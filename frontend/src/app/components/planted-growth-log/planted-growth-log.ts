import { Component, input, signal } from '@angular/core';
import { GrowthLogDto } from '../../models/growth-log.interface';
import { DatePipe } from '@angular/common';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';

@Component({
  selector: 'app-planted-growth-log',
  imports: [DatePipe],
  templateUrl: './planted-growth-log.html',
  styleUrl: './planted-growth-log.css',
})
export class PlantedGrowthLog {
  logs = input<GrowthLogDto[] | undefined>();

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
}
