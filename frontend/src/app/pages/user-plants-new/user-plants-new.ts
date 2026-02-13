import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlaceService } from '../../services/place.service';
import { PlantedDto } from '../../models/planted.interface';
import { PlaceDto } from '../../models/place.interface';
import { ImageDiseasePrediction } from "../../components/image-disease-prediction/image-disease-prediction";
import { AddUpdatePlaceModal } from "../../components/add-update-place-modal/add-update-place-modal";
import { RouterLink } from '@angular/router';
import { PlantGroupService } from '../../services/plant-group.service';
import { PlantGroupDto } from '../../models/plant-group.interface';
import { AddEditGroupModal } from "../../components/add-edit-group-modal/add-edit-group-modal";
import { PLANT_STATUS_MAP, PlantStatusCategory } from '../../enums/plant-status.constants';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-user-plants-new',
  imports: [ImageDiseasePrediction, AddUpdatePlaceModal, RouterLink, AddEditGroupModal, CommonModule, TranslateModule],
  templateUrl: './user-plants-new.html',
  styleUrl: './user-plants-new.css',
})
export class UserPlantsNew implements OnInit, OnDestroy {
  private plantedService = inject(PlantedService);
  private placeService = inject(PlaceService);
  private plantGroupService = inject(PlantGroupService);

  userPlants = signal<PlantedDto[]>([]);
  places = signal<PlaceDto[]>([]);
  plantGroups = signal<PlantGroupDto[]>([]);

  numOfDeadPlants = signal<number>(0);
  isAddPlaceOpen = signal(false);
  isAddGroupOpen = signal(false);
  isCheckDiseaseOpen = signal(false);
  showAllPlaces = signal(false);

  changedGroups = signal(new Map<number, number | null>());

  ngOnInit(): void {
    this.loadUserPlants();
    this.loadPlaces();
    this.loadPlantGroups();
    //this.placeService.loadCountries();
  }

  toggleAddPlaceModal(){
    this.isAddPlaceOpen.update(val => !val);

    if (this.isAddPlaceOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  toggleAddGroupModal(){
    this.isAddGroupOpen.update(val => !val);

    if (this.isAddGroupOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  toggleCheckDiseaseModal(){
    this.isCheckDiseaseOpen.update(val => !val);

    if (this.isCheckDiseaseOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  togglePlaces() {
    this.showAllPlaces.update(v => !v);
  }

  loadPlaces(){
    this.placeService.getAllPlaces().subscribe({
      next: (response) => {
        this.places.set(response);
      },
      error: (err) => {
        this.places.set([]);
        console.log("Error while fetching places", err);
      }
    })
  }

  loadUserPlants(){
    this.plantedService.getAllPlantedPlants().subscribe({
      next: (response) => {
        this.userPlants.set(response.planted);
        this.numOfDeadPlants.set(response.numOfDeadPlants);
      },
      error: (err:any) => {
        this.userPlants.set([]);
        console.log("Error while fetching user planteds", err);
      }
    });
  }

  loadPlantGroups(){
    this.plantGroupService.getAllGroups().subscribe({
      next: (response) => {
        this.plantGroups.set(response);
      },
      error: (err:any) => {
        this.plantGroups.set([]);
        console.log("Error while fetching plant groups", err);
      }
    });
  }

  ngOnDestroy() {
    document.body.classList.remove('overflow-hidden');
  }

  getStatusInfo(statusName?: string) {
    if (!statusName) {
      return this.getDefaultStatus();
    }

    const match = Object.values(PLANT_STATUS_MAP).find(
      status => status.name.toLowerCase() === statusName.toLowerCase()
    ); 

    return match?.color ?? this.getDefaultStatus();
  };

  private getDefaultStatus() {
    return 'bg-gray-100 text-gray-500 border-gray-200';
  }

  onGroupChange(plantId: number, originalGroupId: number | null, event: Event) {
    const select = event.target as HTMLSelectElement;
    const newValue = select.value;
    const newGroupId = newValue === 'null' ? null : Number(newValue);

    const current = new Map(this.changedGroups());

    if (newGroupId === originalGroupId) {
      current.delete(plantId);
    } else {
      current.set(plantId, newGroupId);
    }

    this.changedGroups.set(current);
  }

  saveGroupChange(plantId: number){
    const newGroupId = this.changedGroups().get(plantId);

    if (newGroupId === undefined) return;

    const apiCall = newGroupId === null
    ? this.plantGroupService.removePlantFromGroup(plantId)
    : this.plantGroupService.addPlantToGroup(newGroupId, plantId);

    apiCall.subscribe(() => {
      this.loadUserPlants();
      this.loadPlantGroups();

      const current = new Map(this.changedGroups());
      current.delete(plantId);
      this.changedGroups.set(current);
    });
  }

}
