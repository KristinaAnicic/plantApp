import { Component, computed, effect, inject, input, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PlantService } from '../../services/plant.service';
import { ManyPlantAttributesDto, OnePlantAttributesDto } from '../../models/category.interface';
import { UpsertPlantDto } from '../../models/plant.interface';
import { ImageForm } from '../../models/image.interface';
import { Router } from '@angular/router';
import { ImageUploadService } from '../../services/image-upload.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-add-edit-plant',
  imports: [ReactiveFormsModule],
  templateUrl: './add-edit-plant.html',
  styleUrl: './add-edit-plant.css',
})

export class AddEditPlant implements OnInit{
  private service = inject(PlantService);
  private imageService = inject(ImageUploadService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  singleReference = signal<OnePlantAttributesDto | null>(null);
  multiReference = signal<ManyPlantAttributesDto | null>(null);
  editPlant = input<UpsertPlantDto | null>(null);

  showWarning = signal(false);
  errorMessage = signal('');

  isEditing = computed(() => !!this.editPlant());
  headerText = computed(() => this.isEditing() ? 'Edit Plant' : 'New Botanical Entry');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add plant');

  images = signal<ImageForm[]>([]);

  ngOnInit(): void {
    this.service.getSingleReferenceCategroies()
      .subscribe((response) => {
        this.singleReference.set(response);
        if (!this.isEditing()){
          const firstId = response.timeToFullHeights?.[0]?.id ?? null;
          this.plantForm.get('timeToFullHeightId')?.setValue(firstId);
        }
    });

    this.service.getMultiReferenceCategroies()
      .subscribe((response) => this.multiReference.set(response));
  }

  // update form if existing plant is loaded
  constructor(){
    effect(() => {
      const plant = this.editPlant();
      if (plant) {
        this.plantForm.patchValue(plant);
        const currentImages: ImageForm[] = plant.images.map(image => ({ url: image }))
        this.images.set(currentImages);
      }
      else {
        this.plantForm.reset();
      }
    })
  }

  plantForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    botanicalName: new FormControl('', { nonNullable: true }),
    commonName: new FormControl('', { nonNullable: true }),
    synonymParentPlantId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    fragranceId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    hardinessLevelId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    isSpecie: new FormControl(false, { nonNullable: true}),
    isGenus: new FormControl(false, { nonNullable: true}),
    isPlantForPollinators: new FormControl(false, { nonNullable: true}),
    isLowMaintenance: new FormControl(false, { nonNullable: true}),
    isDroughtResistant: new FormControl(false, { nonNullable: true}),
    spreadTypeId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    heightTypeId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    timeToFullHeightId: new FormControl<number>(0, { nonNullable: true}),
    toxicity: new FormControl('', { nonNullable: true }),
    cultivation: new FormControl('', { nonNullable: true }),
    pestResistance: new FormControl('', { nonNullable: true }),
    diseaseResistance: new FormControl('', { nonNullable: true }),
    pruning: new FormControl('', { nonNullable: true }),   
    propagation: new FormControl('', { nonNullable: true }),
    familyId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    entityDescription: new FormControl('', { nonNullable: true }),
    genusDescription: new FormControl('', { nonNullable: true }),
    
    soilTypes: new FormControl<number[]>([], { nonNullable: true}),
    images: new FormControl<string[]>([], { nonNullable: true}),
    sunlights: new FormControl<number[]>([], { nonNullable: true}),
    aspects: new FormControl<number[]>([], { nonNullable: true}),
    moistures: new FormControl<number[]>([], { nonNullable: true}),
    phs: new FormControl<number[]>([], { nonNullable: true}),
    exposures: new FormControl<number[]>([], { nonNullable: true}),
    habits: new FormControl<number[]>([], { nonNullable: true}),
    seasons: new FormControl<number[]>([], { nonNullable: true}),
  });

  // custom checkbox for arrays
  onCheckBoxChange(controlName: string, id: number){
    const control = this.plantForm.get(controlName);
    const currentValues: number[] = Array.isArray(control?.value) ? control.value : [];
    let newValues: number[];

    if (currentValues.includes(id)){
      newValues = currentValues.filter(val => val !== id);
    }
    else {
      newValues = [...currentValues, id];
    }

    control?.setValue(newValues, { emitEvent: true });
    control?.markAsDirty();
    control?.updateValueAndValidity();
  }

  isSelected(controlName: string, id: number): boolean {
    const control = this.plantForm.get(controlName);
    const currentValues = control?.value || [];

    return currentValues.includes(id) ? true : false;
  }

  onFilesSelected(event: any){
    const files: FileList = event.target.files;
    if (!files) return;

    Array.from(files).forEach(file => {
      const reader = new FileReader();
      reader.onload = (r: any) => {
        const newImage: ImageForm = { url: r.target.result, file: file }
        this.images.update(current => [...current, newImage]);
      }
      reader.readAsDataURL(file);
    });

    event.target.value = '';
  }

  removeImage(index: number) {
    this.images.update(current => current.filter((_, i) => i !== index));
  }

  addImageUrl(url: string){
    if (!url || !url.trim()) return;

    const newImage: ImageForm = { url: url.trim() };
    this.images.update(curr => [...curr, newImage]);
  }

  async addEditPlant(){
    const data: UpsertPlantDto = this.plantForm.getRawValue();

    const imageUrls: string[] = this.images().filter(im => !im.file).map(im => im.url);

    const imagesToUpload = this.images()
      .filter(img => !!img.file);
    const uploadedImages: string[] = (await this.imageService.uploadImages(imagesToUpload))
      .map(im => im.serverUrl);

    const allImageUrls = [...imageUrls, ...uploadedImages];

    const cleanData: UpsertPlantDto = {
      ...data,
      entityDescription: data.entityDescription?.trim() || undefined,
      cultivation: data.cultivation?.trim() || undefined,
      propagation: data.propagation?.trim() || undefined,
      pruning: data.pruning?.trim() || undefined,
      pestResistance: data.pestResistance?.trim() || undefined,
      diseaseResistance: data.diseaseResistance?.trim() || undefined,
      toxicity: data.toxicity?.trim() || undefined,
      hardinessLevelId: data.hardinessLevelId ?? undefined,
      familyId: data.familyId ?? undefined,
      synonymParentPlantId: data.synonymParentPlantId ?? undefined,
      fragranceId: data.fragranceId ?? undefined,
      spreadTypeId: data.spreadTypeId ?? undefined,
      heightTypeId: data.heightTypeId ?? undefined,

      images: allImageUrls ?? undefined
    };

    if (this.isEditing()){
      const id = this.editPlant()?.id;

      console.log(id);
      if (id === undefined) {
        console.error("Cannot save changes for plant without providing id.");
        return;
      }
      
      this.service.updatePlant(id, cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully saved changes');   
          this.router.navigate(['/plant', id]);
        },
        error:(err:any) => {
          this.handleError(err, "Error on editing plant");
        }
      })
    }
    
    else {
      this.service.addPlant(cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully added plant');
          this.router.navigate(['']);
        },
        error:(err:any) => {
          this.handleError(err, "Error on adding plant");
        }
      })
    }
  }

  private handleError(err: any, logMessage: string) {
    this.showWarning.set(true);
    this.errorMessage.set(err.error?.error || "An unexpected error occurred");
    console.log(logMessage, err);
  }
}
