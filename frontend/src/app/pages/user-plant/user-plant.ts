import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedGetDto } from '../../models/planted.interface';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { DatePipe } from '@angular/common';
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";

@Component({
  selector: 'app-user-plant',
  imports: [DatePipe, PlantedGrowthLog, PlantedReminders],
  templateUrl: './user-plant.html',
  styleUrl: './user-plant.css',
})
export class UserPlant implements OnInit {
  @Input() id!: string;

  service = inject(PlantedService);
  planted = signal<PlantedGetDto | null>(null);

  displayImages = computed(() => {
    const all = this.planted()?.images ?? [];
    return all.length > 4 ? all.slice(0, 3) : all.slice(0, 4);
  })

  hasMoreImages = computed(() => (this.planted()?.images?.length ?? 0) > 4);

  plantName = computed(() => {
    const planted = this.planted();
    if (!planted) return;

    return [
      planted.plant.commonName,
      planted.plant.botanicalName
    ]
    .filter(val => !!val)
    .join(' • ');
  });

  ngOnInit(): void {
    this.service.getPlanted(parseInt(this.id)).subscribe({
      next: (result) => {
        this.planted.set(result);
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }

  statusInfo = computed(() => {
    const statusId = this.planted()?.plantStatus?.id;

    if (statusId && PLANT_STATUS_MAP[statusId]) {
    return PLANT_STATUS_MAP[statusId];
    }

    return { 
      name: 'Not specified', 
      category: PlantStatusCategory.Inactive, 
      color: 'bg-gray-100 text-gray-500 border-gray-200' 
    };
  });
}
