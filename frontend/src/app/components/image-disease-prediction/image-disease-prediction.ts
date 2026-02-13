import { Component, inject, output, signal } from '@angular/core';
import { DiseaseService } from '../../services/disease.service';
import { DiseasePredictionResponse } from '../../models/disease-prediction-response.interface';
import { NotificationService } from '../../services/notification.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-image-disease-prediction',
  imports: [TranslateModule],
  templateUrl: './image-disease-prediction.html',
  styleUrl: './image-disease-prediction.css',
})
export class ImageDiseasePrediction {
  close = output<void>();
  service = inject(DiseaseService);
  private notificationService = inject(NotificationService);
  
  prediction = signal<DiseasePredictionResponse | null>(null);
  loadedImage = signal<string | null>(null);
  selectedFile = signal<File | null>(null);
  showWarning = signal(false);
  isLoading = signal(false);
  errorMessage = signal('');

  onFileSelected(event: any){
    const file: File = event.target.files[0];
    if (!file) return;

    this.selectedFile.set(file);
    const reader = new FileReader();
    reader.onload = (r: any) => {
      this.loadedImage.set(r.target.result);
    }
    reader.readAsDataURL(file);
    event.target.value = '';
  }

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  onUploadClick(){
    const file = this.selectedFile();
    if (!file) return;
    this.isLoading.set(true);
    this.service.predictDisease(file).subscribe({
      next: (res) => {
        this.prediction.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notificationService.showError(err.error.error);
        console.log("Error on adding planted: ", err);
      }
    })
  }
}
