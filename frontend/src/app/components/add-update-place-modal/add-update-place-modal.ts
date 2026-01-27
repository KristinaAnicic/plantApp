import { Component, computed, effect, inject, input, OnInit, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PlaceService } from '../../services/place.service';
import { UpsertPlaceDto } from '../../models/place.interface';
import { toSignal } from '@angular/core/rxjs-interop';
import { SUN_LEVELS } from '../../constants/sunlight.constants';
import { HUMIDITY_LEVELS } from '../../constants/humidity.constants';

@Component({
  selector: 'app-add-update-place-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './add-update-place-modal.html',
  styleUrl: './add-update-place-modal.css',
})
export class AddUpdatePlaceModal implements OnInit {
  close = output<void>();
  placeAdded = output<void>();
  editPlace = input<UpsertPlaceDto | null>(null);

  placeService = inject(PlaceService);
  countries = this.placeService.countries;
  showWarning = signal(false);
  errorMessage = signal('');

  sunLevels = SUN_LEVELS;
  humidityLevels = HUMIDITY_LEVELS;

  isEditing = computed(() => !!this.editPlace());
  headerText = computed(() => this.isEditing() ? 'Edit location' : 'Add a new location');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add place');

  placeForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    name: new FormControl('', { nonNullable: true }),
    address: new FormControl('', { nonNullable: true }),
    city: new FormControl('', { nonNullable: true }),
    note: new FormControl('', { nonNullable: true }),
    countryId: new FormControl(0, { nonNullable: true }),
    sunlightIntensity: new FormControl<number>(3, { nonNullable: true }),
    humidityIntensity: new FormControl<number>(3, { nonNullable: true })
  });

  private sunlightChanges = toSignal(
    this.placeForm.get('sunlightIntensity')!.valueChanges, 
    { initialValue: 3 }
  );

  private humidityChanges = toSignal(
    this.placeForm.get('humidityIntensity')!.valueChanges, 
    { initialValue: 3 }
  );

  ngOnInit(): void {
    const place = this.editPlace();
    const countries = this.countries();

    if (place) {
      this.placeForm.patchValue(place);
    }
    else {
      if (countries.length > 0) {
        this.placeForm.patchValue({ 
          countryId: countries[0].id 
        });
      }
    }
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

  selectedSun = computed(() => this.sunlightChanges());
  selectedHumidity = computed(() => this.humidityChanges());
}
