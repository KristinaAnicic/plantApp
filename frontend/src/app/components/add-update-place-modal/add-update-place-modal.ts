import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PlaceService } from '../../services/place.service';
import { UpsertPlaceDto } from '../../models/place.interface';

@Component({
  selector: 'app-add-update-place-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './add-update-place-modal.html',
  styleUrl: './add-update-place-modal.css',
})
export class AddUpdatePlaceModal {
  close = output<void>();
  placeAdded = output<void>();
  editPlace = input<UpsertPlaceDto | null>(null);

  placeService = inject(PlaceService);
  countries = this.placeService.countries;
  showWarning = signal(false);
  errorMessage = signal('');

  isEditing = computed(() => !!this.editPlace());
  headerText = computed(() => this.isEditing() ? 'Edit location' : 'Add a new location');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add place');

  placeForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    name: new FormControl('', { nonNullable: true }),
    address: new FormControl('', { nonNullable: true }),
    city: new FormControl('', { nonNullable: true }),
    note: new FormControl('', { nonNullable: true }),
    countryId: new FormControl(this.countries()[0].id, { nonNullable: true })
  });

  constructor() {
    effect(() => {
      const place = this.editPlace();
      if (place) {
        this.placeForm.patchValue(place);
      }
      else {
        this.placeForm.reset();
      }
    })
  }

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  addEditPlace(){
    const data: UpsertPlaceDto = this.placeForm.getRawValue();
    const cleanData: UpsertPlaceDto = {
      ...data,
      name: data.name.trim(),
      address: data.address?.trim(),
      city: data.city.trim()
    };

    if (this.isEditing()){
      const id = this.editPlace()?.id;

      if (id === undefined) {
        console.error("Cannot save changes for place without providing id.");
        return;
      }

      this.placeService.updatePlace(id, cleanData).subscribe({
        next:() => {
          this.placeAdded.emit();
          this.onCloseClick();
        },
        error:(err:any) => {
          this.showWarning.set(true);
          this.errorMessage.set(err.error.error);
          console.log("Error on editing place", err);
        }
      })
    }
    
    else {
      this.placeService.addPlace(cleanData).subscribe({
        next:() => {
          this.placeAdded.emit();
          this.onCloseClick();
        },
        error:(err:any) => {
          this.showWarning.set(true);
          this.errorMessage.set(err.error.error);
          console.log("Error on adding a place", err);
        }
      })
    }
    
  }
}
