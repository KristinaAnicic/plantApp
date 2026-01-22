import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedGetDto, UpsertPlantedDto } from '../../models/planted.interface';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";
import { AddEditPlantedModal } from "../../components/add-edit-planted-modal/add-edit-planted-modal";

@Component({
  selector: 'app-user-plant',
  imports: [PlantedGrowthLog, PlantedReminders, AddEditPlantedModal],
  templateUrl: './user-plant.html',
  styleUrl: './user-plant.css',
})
export class UserPlant implements OnInit {
  @Input() id!: string;

  service = inject(PlantedService);
  planted = signal<PlantedGetDto | null>(null);
  isEditPlantedModalOpen = signal(false);
  plantedToEdit = signal<UpsertPlantedDto | null>(null);

  displayImages = computed(() => {
    const all = this.planted()?.images?.filter(im => im.url !== this.planted()?.image) ?? [];
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
    this.loadPlanted();
  }

  loadPlanted(){
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

  toggleAddPlantedModal(){
    this.isEditPlantedModalOpen.update(val => !val);
  }

  editPlanted(){
    const currentPlanted = this.planted();
    if (!currentPlanted) return;

    const plant: UpsertPlantedDto = {
      ...currentPlanted,
      plantId: currentPlanted.plant.plantId,
      placeId: currentPlanted.place.id,
      plantStatusId: currentPlanted.plantStatus?.id ?? 0,
      datePlanted: currentPlanted.datePlanted.split('T')[0],
      images: currentPlanted.images?.map(im => im.url) ?? []
    }
    this.plantedToEdit.set(plant);
    this.isEditPlantedModalOpen.set(true);
  }
}
