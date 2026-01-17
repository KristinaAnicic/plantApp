import { Component, computed, inject, Input, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { PlaceService } from '../../services/place.service';
import { PlaceGetDto, UpsertPlaceDto } from '../../models/place.interface';
import { PlantedService } from '../../services/planted.service';
import { AddUpdatePlaceModal } from "../../components/add-update-place-modal/add-update-place-modal";
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-place-plants',
  imports: [RouterLink, AddUpdatePlaceModal],
  templateUrl: './place-plants.html',
  styleUrl: './place-plants.css',
})
export class PlacePlants implements OnInit, OnDestroy {
  @Input() id!: string;
  
  private plantedService = inject(PlantedService);
  private placeService = inject(PlaceService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  place = signal<PlaceGetDto | null>(null);
  placeToEdit = signal<UpsertPlaceDto | null>(null);
  isEditModalOpen = signal(false);
  
  location = computed(() => {
    const currentPlace = this.place();
    if (!currentPlace) return;

    return [
      currentPlace.address,
      currentPlace.city,
      currentPlace.country.name
    ]
    .filter(val => !!val)
    .join(', ');
  })

  ngOnInit(): void {
    this.loadPlace();
  }

  editPlace(){
    const currentPlace = this.place();
    if (!currentPlace) return;

    this.placeToEdit.set({
      id: currentPlace.id,
      name: currentPlace.name,
      address: currentPlace.address ?? '',
      city: currentPlace.city ?? '',
      note: currentPlace.note ?? '',
      countryId: currentPlace.country.id
    });

    this.isEditModalOpen.set(true);
  }

  deletePlace(){
    const id = this.place()?.id;
    if (id === undefined)
      return

    this.placeService.removePlace(id).subscribe({
      next: () => {
        this.router.navigate(['/my-plants'])

        this.snackBar.open('Successfully removed place', 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['snackbar-success']
        });
      },
      error: (err) => {
        this.snackBar.open(err.error.error, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['snackbar-error']
        });
      }
    });
  }

  togglePlaceModal(){
    this.isEditModalOpen.update(val => !val);

    if (this.isEditModalOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  loadPlace(){
    this.plantedService.getAllPlantedPlantsByPlaceId(parseInt(this.id)).subscribe({
      next: (response) => {
        this.place.set(response);
      },
      error: (err:any) => {
        this.place.set(null);
        console.log("Error while fetching planteds by place", err);
      }
    });
  }

  ngOnDestroy() {
    document.body.classList.remove('overflow-hidden');
  }
}
