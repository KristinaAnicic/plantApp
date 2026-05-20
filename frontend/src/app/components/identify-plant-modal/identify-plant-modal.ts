import { Component, inject, output, signal } from '@angular/core';
import { PlantService } from '../../services/plant.service';
import { NotificationService } from '../../services/notification.service';
import { PlantNetResponse, PlantNetResult } from '../../models/plant-net.interface';
import { ImageForm } from '../../models/image.interface';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-identify-plant-modal',
  imports: [TranslateModule],
  templateUrl: './identify-plant-modal.html',
  styleUrl: './identify-plant-modal.css',
})
export class IdentifyPlantModal {
  close = output<void>();
  service = inject(PlantService);
  private notificationService = inject(NotificationService);
  
  results = signal<PlantNetResponse | null>(null);
  result = signal<PlantNetResult | null>(null);
  images = signal<ImageForm[]>([]);
  showWarning = signal(false);
  isLoading = signal(false);
  errorMessage = signal('');

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

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  onUploadClick(){
    const images = this.images()
      .map(f => f.file)
      .filter((file): file is File => file !== undefined);;
    if (!images.length) return;

    this.isLoading.set(true);
    this.service.identifyPlant(images).subscribe({
      next: (res) => {
        this.results.set(res);
        this.result.set(res.results[0]);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notificationService.showError(err.error.error);
        console.log("Error on identifying plant: ", err);
        this.isLoading.set(false);
      }
    })
  }

  removeImage(index: number) {
    this.images.update(current => current.filter((_, i) => i !== index));
  }
}
