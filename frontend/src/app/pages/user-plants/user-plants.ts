import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedDto } from '../../models/planted.interface';
import { RouterLink } from '@angular/router';
import { PlaceService } from '../../services/place.service';
import { PlaceDto } from '../../models/place.interface';
import { AddUpdatePlaceModal } from "../../components/add-update-place-modal/add-update-place-modal";
import { ImageDiseasePrediction } from "../../components/image-disease-prediction/image-disease-prediction";

@Component({
  selector: 'app-user-plants',
  imports: [RouterLink, AddUpdatePlaceModal, ImageDiseasePrediction],
  templateUrl: './user-plants.html',
  styleUrl: './user-plants.css',
})
export class UserPlants implements OnInit, OnDestroy {
  private plantedService = inject(PlantedService);
  private placeService = inject(PlaceService);
  userPlants = signal<PlantedDto[]>([]);
  places = signal<PlaceDto[]>([]);
  numOfDeadPlants = signal<number>(0);
  isAddPlaceOpen = signal(false);
  isCheckDiseaseOpen = signal(false);

  ngOnInit(): void {
    this.loadUserPlants();
    this.loadPlaces();
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

  toggleCheckDiseaseModal(){
    this.isCheckDiseaseOpen.update(val => !val);

    if (this.isCheckDiseaseOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
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

  ngOnDestroy() {
    document.body.classList.remove('overflow-hidden');
  }
}
