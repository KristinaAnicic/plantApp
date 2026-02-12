import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantGroupService } from '../../services/plant-group.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { PlantGroupGetDto, UpsertPlantGroupDto } from '../../models/plant-group.interface';
import { UpsertGrowthLogDto } from '../../models/growth-log.interface';
import { UpsertReminderDto } from '../../models/reminder.interface';
import { AddEditLogModal } from "../../components/add-edit-log-modal/add-edit-log-modal";
import { AddEditGroupModal } from "../../components/add-edit-group-modal/add-edit-group-modal";
import { PlantedGrowthLog } from "../../components/planted-growth-log/planted-growth-log";
import { PlantGroupList } from "../../components/plant-group-list/plant-group-list";
import { PlantedReminders } from "../../components/planted-reminders/planted-reminders";

@Component({
  selector: 'app-plant-group',
  imports: [AddEditLogModal, AddEditGroupModal, PlantedGrowthLog, PlantGroupList, PlantedReminders],
  templateUrl: './plant-group.html',
  styleUrl: './plant-group.css',
})
export class PlantGroup implements OnInit {
  @Input() id!: string;
  currentGroupId = signal<number | null>(null);

  service = inject(PlantGroupService);
  private router = inject(Router);
  public notif = inject(NotificationService);
  private route = inject(ActivatedRoute);

  group = signal<PlantGroupGetDto | null>(null);
  isEditGroupModalOpen = signal(false);
  isLogModalOpen = signal(false);
  isReminderModalOpen = signal(false);
  isManagePlantsModalOpen = signal(false);
  groupToEdit = signal<UpsertPlantGroupDto | null>(null);
  logToEdit = signal<UpsertGrowthLogDto | null>(null);
  reminderToEdit = signal<UpsertReminderDto | null>(null);
  showOnlyGroupLogs = signal(false);

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      this.currentGroupId.set(id);
      this.loadGroup();
    })
  }

  loadGroup(){
    this.isManagePlantsModalOpen.set(false);
    const id = this.currentGroupId();
    if (!id) return;

    this.service.getGroup(id).subscribe({
      next: (result) => {
        this.group.set(result);
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }


  toggleAddGroupModal(){
    this.isEditGroupModalOpen.update(val => !val);
  }

  toggleLogModal(){
    this.isLogModalOpen.update(val => !val);
  }

  toggleReminderModal(){
    this.isReminderModalOpen.update(val => !val);
  }

  togglePlantsModal(){
    this.isManagePlantsModalOpen.update(val => !val);
  }

  toggleShowLogsModal(){
    this.showOnlyGroupLogs.update(val => !val);
  }

  editGroup(){
    const currentPlanted = this.group();
    if (!currentPlanted) return;

    const plant: UpsertPlantGroupDto = {
      ...currentPlanted
    }
    this.groupToEdit.set(plant);
    this.isEditGroupModalOpen.set(true);
  }

  editLog(id: number){
    const log = this.group()?.growthLogs?.find(g => g.id === id);
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
    const reminder = this.group()?.reminders?.find(g => g.id === id);
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

  deleteGroup(){
    this.service.removeGroup(parseInt(this.id)).subscribe({
      next: () => {
        this.router.navigate(['/my-plants']);
        this.notif.showSuccess("Successfully deleted plant")
      },
      error: () => this.notif.showError("Couldn't remove plant, try again later!")
    });
  }

  openPlanted(id: number){
    this.router.navigate(['my-plants', id])
  }

  filteredlogs = computed(() => {
    const logs = this.group()?.growthLogs ?? [];

    if (this.showOnlyGroupLogs()){
      return logs.filter(l => l.plantedId === null)
    }
    return logs;
  })
}
