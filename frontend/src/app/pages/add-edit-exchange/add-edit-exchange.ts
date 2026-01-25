import { Component, computed, effect, inject, input, OnInit, signal, untracked } from '@angular/core';
import { PlantExchangeReference, UpsertPlantExchangeDto } from '../../models/plant-exchange.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ImageForm } from '../../models/image.interface';
import { ImageUploadService } from '../../services/image-upload.service';
import { NotificationService } from '../../services/notification.service';
import { Router } from '@angular/router';
import { PlantExchangeService } from '../../services/plant-exchange.service';
import { PlaceService } from '../../services/place.service';
import { ExchangeType } from '../../enums/plant-exchange-type.enum';

@Component({
  selector: 'app-add-edit-exchange',
  imports: [ReactiveFormsModule],
  templateUrl: './add-edit-exchange.html',
  styleUrl: './add-edit-exchange.css',
})
export class AddEditExchange implements OnInit {
  editTrade = input<UpsertPlantExchangeDto | null>(null);
  private service = inject(PlantExchangeService);
  private placeService = inject(PlaceService);
  private imageService = inject(ImageUploadService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  references = signal<PlantExchangeReference | null>(null);
  countries = this.placeService.countries;

  isEditing = computed(() => !!this.editTrade());
  headerText = computed(() => this.isEditing() ? 'Edit Plant Exchange' : 'Create New Plant Exchange');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Create listing');

  images = signal<ImageForm[]>([]);

  ngOnInit(): void {
    this.service.getReferences().subscribe((response) => {
      this.references?.set(response);

      const trade = this.editTrade();
      const countries = this.countries();

      if (trade) {
        this.tradeForm.patchValue(trade);
        const currentImages: ImageForm[] = trade.images.map(image => ({ url: image }))
        untracked(() => this.images.set(currentImages));
      }
      else {
        this.tradeForm.patchValue({
          exchangeTypeId: response.exchangeTypes.find(e => e.name.toLowerCase() === 'free')?.id,
          countryId: countries.length > 0 ? countries[0].id : undefined 
        });
      }
    })
  }
  
  tradeForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    title: new FormControl('', { nonNullable: true }),
    content: new FormControl('', { nonNullable: true }),
    plantStatus: new FormControl('', { nonNullable: true }),
    contact: new FormControl('', { nonNullable: true }),
    mainImage: new FormControl('', { validators: Validators.required, nonNullable: true }),
    city: new FormControl('', { nonNullable: true }),
    exchangeFor: new FormControl<string | undefined>('', { nonNullable: true }),
    price: new FormControl<number | undefined>(0, { nonNullable: true }),
    shipping: new FormControl('', { nonNullable: true }),
    plantedId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    isActive: new FormControl(true, { nonNullable: true}),
    exchangeTypeId: new FormControl<number | undefined>(undefined, { nonNullable: true}),
    countryId: new FormControl<number>(0, { nonNullable: true}),
    
    images: new FormControl<string[]>([], { nonNullable: true}),
  })


  async addEditExchange(){
    this.resetValues();
    if(this.tradeForm.invalid) return;

    const data = this.tradeForm.getRawValue();   
    const finalImages = await this.imageService.prepareImages(data.mainImage, this.images());

    if (!finalImages.mainImage?.trim()) return;
    const cleanData: UpsertPlantExchangeDto = {
      ...data, 
      exchangeTypeId: data.exchangeTypeId ?? 0,
      mainImage: finalImages.mainImage || "",
      images: finalImages.images ?? undefined,
    };
  
    if (this.isEditing()){
      const id = this.editTrade()?.id;

      console.log(id);
      if (id === undefined) {
        console.error("Cannot save changes for plant without providing id.");
        return;
      }
      
      this.service.updatePlantExchange(id, cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully saved changes');   
          this.router.navigate(['/trade', id]);
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on editing planted: ", err);
        }
      })
    }
    
    else {
      this.service.addPlantExchange(cleanData).subscribe({
        next:() => {
          this.notificationService.showSuccess('Successfully added planted');
          this.router.navigate(['/trade']);
        },
        error:(err:any) => {
          this.notificationService.showError(err.error.error);
          console.log("Error on adding planted: ", err);
        }
      })
    }
  }

  resetValues() {
    const currentTypeId = this.tradeForm.get('exchangeTypeId')?.value;
    if (currentTypeId === ExchangeType.Sell)
      this.tradeForm.get('exchangeFor')?.setValue(undefined)

    else if (currentTypeId === ExchangeType.Swap)
      this.tradeForm.get('price')?.setValue(undefined);

    else if (currentTypeId === ExchangeType.Free){
      this.tradeForm.patchValue({
        exchangeFor: undefined,
        price: 0
      });
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
    const control = this.tradeForm.get('mainImage');
    if (!control) return;

    const newValue = url;
    control.setValue(newValue);
  }
}
