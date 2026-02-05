import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedGetDto, UpsertPlantedDto } from '../../models/planted.interface';
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";
import { AddEditPlantedModal } from "../../components/add-edit-planted-modal/add-edit-planted-modal";
import { AddEditLogModal } from "../../components/add-edit-log-modal/add-edit-log-modal";
import { GrowthLogGetDto, UpsertGrowthLogDto } from '../../models/growth-log.interface';
import { UpsertReminderDto } from '../../models/reminder.interface';
import { AddEditReminderModal } from "../../components/add-edit-reminder-modal/add-edit-reminder-modal";
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-user-plant',
  imports: [PlantedGrowthLog, PlantedReminders, AddEditPlantedModal, AddEditLogModal, AddEditReminderModal],
  templateUrl: './user-plant.html',
  styleUrl: './user-plant.css',
})
export class UserPlant implements OnInit {
  @Input() id!: string;

  service = inject(PlantedService);
  private router = inject(Router);
  public notif = inject(NotificationService);

  planted = signal<PlantedGetDto | null>(null);
  isEditPlantedModalOpen = signal(false);
  isLogModalOpen = signal(false);
  isReminderModalOpen = signal(false);
  plantedToEdit = signal<UpsertPlantedDto | null>(null);
  logToEdit = signal<UpsertGrowthLogDto | null>(null);
  reminderToEdit = signal<UpsertReminderDto | null>(null);

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

  toggleLogModal(){
    this.isLogModalOpen.update(val => !val);
  }

  toggleReminderModal(){
    this.isReminderModalOpen.update(val => !val);
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

  editLog(id: number){
    const log = this.planted()?.growthLogs?.find(g => g.id === id);
    if (!log) return;

    const editLog: UpsertGrowthLogDto = {
      ...log,
      plantStatusId: log.plantStatus?.id ?? 0,
      images: log.images?.map(im => im.url) ?? []
    }
    this.logToEdit.set(editLog);
    this.isLogModalOpen.set(true);
  }

  addLog(){
    this.logToEdit.set(null);
    this.isLogModalOpen.set(true);
  }

  editReminder(id: number){
    const reminder = this.planted()?.nextReminders?.find(g => g.id === id);
    if (!reminder) return;

    const editReminder: UpsertReminderDto = {
      ...reminder,
      frequencyTypeId: reminder.frequencyType?.id ?? 0,
      reminderTypeId: reminder.reminderType?.id ?? 0
    }
    this.reminderToEdit.set(editReminder);
    this.isReminderModalOpen.set(true);
  }

  addReminder(){
    this.reminderToEdit.set(null);
    this.isReminderModalOpen.set(true);
  }

  deletePlanted(){
    this.service.removePlanted(parseInt(this.id)).subscribe({
      next: () => {
        this.router.navigate(['/my-plants']);
        this.notif.showSuccess("Successfully deleted plant")
      },
      error: () => this.notif.showError("Couldn't remove plant, try again later!")
    });
  }
}
