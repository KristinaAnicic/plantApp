import { Component, computed, effect, inject, input, output, signal, untracked } from '@angular/core';
import { GrowthLogService } from '../../services/growth-log.service';
import { ImageUploadService } from '../../services/image-upload.service';
import { NotificationService } from '../../services/notification.service';
import { UpsertGrowthLogDto } from '../../models/growth-log.interface';
import { ImageForm } from '../../models/image.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PlantedService } from '../../services/planted.service';

@Component({
  selector: 'app-add-edit-log-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './add-edit-log-modal.html',
  styleUrl: './add-edit-log-modal.css',
})
export class AddEditLogModal {
  private service = inject(GrowthLogService);
  private plantedService = inject(PlantedService);
  private imageService = inject(ImageUploadService);
  private notificationService = inject(NotificationService);

  editLog = input<UpsertGrowthLogDto | null>(null);
  plantedId = input<number | null>(null);
  logEdited = output<void>();
  close = output<void>();

  references = this.plantedService.references;
  showWarning = signal(false);
  errorMessage = signal('');
  images = signal<ImageForm[]>([]);

  isEditing = computed(() => !!this.editLog());
  headerText = computed(() => this.isEditing() ? 'Edit Growth Log Entry' : 'Add New Growth Log Entry');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add log');

  logForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    title: new FormControl<string>('', {validators: [Validators.required, Validators.pattern(/[\S]/)], nonNullable: true }),
    observationDate: new FormControl(new Date().toISOString().split('T')[0], { nonNullable: true }),
    note: new FormControl('', { nonNullable: true }),
    plantStatusId: new FormControl<number | undefined>(0, { nonNullable: true }),
    plantedId: new FormControl<number | undefined>(0, { nonNullable: true }),
    images: new FormControl<string[]>([])
  })

  constructor() {
    effect(() => {
      const log = this.editLog();
      const refs = this.references();

      if (refs && refs.plantStatuses.length > 0){
        if (log) {
          this.logForm.patchValue(log, { emitEvent: false });
          const currentImages: ImageForm[] = log.images.map(image => ({ url: image }))
          untracked(() => {
            this.images.set(currentImages);
          });
        }
        else {
          this.logForm.reset();
          untracked(() => this.images.set([]));
          const currentStatus = this.logForm.get('plantStatusId')?.value;
          if (!currentStatus || currentStatus === 0) {
            this.logForm.patchValue({ 
              plantStatusId: refs.plantStatuses[0].id,
              plantedId: this.plantedId() ?? 0
            }, { emitEvent: false });
          }
        }
      }
      
    })
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

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  async addEditLog(){
    if (this.logForm.invalid) return;

    const data = this.logForm.getRawValue();   
    const finalImages = await this.imageService.prepareImages(data, this.images());

    const cleanData: UpsertGrowthLogDto = {
      ...data, 
      plantedId: Number(data.plantedId),
      plantStatusId: Number(data.plantStatusId),
      observationDate: data.observationDate || undefined,
      title: data.title?.trim() || "", 
      note: data.note?.trim() || undefined,
      images: finalImages.images ?? undefined,
    };
  
    if (this.isEditing()){
      const id = this.editLog()?.id;

      console.log(id);
      if (id === undefined) {
        console.error("Cannot save changes for plant without providing id.");
        return;
      }
      
      this.service.updateLog(id, cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully saved changes');   
          this.logEdited.emit();
          this.close.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on editing planted: ", err);
        }
      })
    }
    
    else {
      this.service.addLog(cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully added planted');
          this.close.emit();
          this.logEdited.emit();
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on adding planted: ", err);
        }
      })
    }
  }
  
}
