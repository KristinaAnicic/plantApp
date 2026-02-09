import { Component, computed, effect, inject, input, OnInit, output, signal, untracked } from '@angular/core';
import { PlantedReference, UpsertPlantedDto } from '../../models/planted.interface';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PlantedService } from '../../services/planted.service';
import { ImageForm, UploadMapping } from '../../models/image.interface';
import { ImageUploadService } from '../../services/image-upload.service';
import { NotificationService } from '../../services/notification.service';
import { AddUpdatePlaceModal } from "../add-update-place-modal/add-update-place-modal";

@Component({
  selector: 'app-add-edit-planted-modal',
  imports: [ReactiveFormsModule, AddUpdatePlaceModal],
  templateUrl: './add-edit-planted-modal.html',
  styleUrl: './add-edit-planted-modal.css',
})
export class AddEditPlantedModal implements OnInit {
  private service = inject(PlantedService);
  private imageService = inject(ImageUploadService);
  private notificationService = inject(NotificationService);

  editPlanted = input<UpsertPlantedDto | null>(null);
  plantId = input<number | null>(null);
  plantedEdited = output<void>();
  close = output<void>();

  references = signal<PlantedReference | null>(null);
  showWarning = signal(false);
  errorMessage = signal('');
  images = signal<ImageForm[]>([]);

  isEditing = computed(() => !!this.editPlanted());
  headerText = computed(() => this.isEditing() ? 'Edit Planted' : 'Add Planted Specie');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add planted');

  ngOnInit(): void {
    if (!this.editPlanted() && !this.plantId()) {
      throw new Error('AddEditPlantedModal requires [editPlanted] for editing or [plantId] for adding.');
    }

    this.setReferences();
  }
  
  setReferences(){
    this.service.getReferences().subscribe((response) => {
      this.references?.set(response)
      const planted = this.editPlanted();

      if (planted) {
        this.plantedForm.patchValue(planted);
        const currentImages: ImageForm[] = planted.images.map(image => ({ url: image }));
        this.images.set(currentImages);
      }
      else {
        this.plantedForm.patchValue({ 
          plantStatusId: response.plantStatuses[0]?.id,
          plantId: this.plantId()
        });

        this.images.set([]);
      }
    });
  }

  plantedForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    name: new FormControl<string | undefined>('', { nonNullable: true }),
    plantId: new FormControl<number | null>(this.plantId(), { nonNullable: true }),
    placeId: new FormControl<number | string>("", { nonNullable: true }),
    datePlanted: new FormControl(new Date().toISOString().split('T')[0], { nonNullable: true }),
    source: new FormControl('', { nonNullable: true }),
    note: new FormControl('', { nonNullable: true }),
    isOutside: new FormControl(false, { nonNullable: true }),
    image: new FormControl<string | undefined>('', { nonNullable: true }),
    plantStatusId: new FormControl<number | undefined>(0, { nonNullable: true }),
    images: new FormControl<string[]>([])
  })

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  async addEditPlanted(){
    const data = this.plantedForm.getRawValue();   
    const finalImages = await this.imageService.prepareImages(data.image, this.images());

    const cleanData: UpsertPlantedDto = {
      ...data, 
      plantId: Number(data.plantId ?? 0),
      placeId: Number(data.placeId),
      plantStatusId: Number(data.plantStatusId),
      datePlanted: data.datePlanted || undefined,
      name: data.name?.trim() || undefined, 
      source: data.source?.trim() || undefined,
      note: data.note?.trim() || undefined,
      image: finalImages.mainImage || undefined,
      images: finalImages.images ?? undefined,
    };
  
    if (this.isEditing()){
      const id = this.editPlanted()?.id;

      console.log(id);
      if (id === undefined) {
        console.error("Cannot save changes for plant without providing id.");
        return;
      }
      
      this.service.updatePlanted(id, cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully saved changes');   
          this.plantedEdited.emit();
          this.close.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on editing planted: ", err);
        }
      })
    }
    
    else {
      this.service.addPlanted(cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully added planted');
          this.close.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on adding planted: ", err);
        }
      })
    }
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

  changeMainImage(url: string){
    const control = this.plantedForm.get('image');
    if (!control) return;

    const newValue = control.value === url ? '' : url;
    control.setValue(newValue);
  }
}
