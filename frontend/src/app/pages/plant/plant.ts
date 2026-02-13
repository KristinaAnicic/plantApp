import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantService } from '../../services/plant.service';
import { PlantGetDto } from '../../models/plant.interface';
import { Reference } from '../../models/reference.interface';
import { Router, RouterLink } from "@angular/router";
import { AuthService } from '../../services/auth.service';
import { AddEditPlantedModal } from "../../components/add-edit-planted-modal/add-edit-planted-modal";
import { NotificationService } from '../../services/notification.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-plant',
  imports: [AddEditPlantedModal, TranslateModule],
  templateUrl: './plant.html',
  styleUrl: './plant.css',
})
export class Plant implements OnInit {
  @Input() id!: string;
  
  private service = inject(PlantService);
  private router = inject(Router);
  public authService = inject(AuthService);
  public notif = inject(NotificationService);
  private translate = inject(TranslateService);

  plant = signal<PlantGetDto | null>(null);
  selectedImage = signal<string | null>(null);
  selectedOption = signal<string | null>(null);
  isAddPlantedModalOpen = signal(false);
  isOptionsMenuOpened = signal(false);

  ngOnInit(): void {
    this.service.getPlant(parseInt(this.id)).subscribe({
      next: (response) => {
        this.plant.set(response);

        const availableOptions = this.options();
        if (availableOptions.length > 0) {
          this.selectedOption.set(availableOptions[0]);
        }
      },
      error: (err: any) => {
        this.plant.set(null);
        console.log("Error while fetching plant", err);
      }
    })  
  }

  scroll(container: HTMLElement, direction: number){
    const scrollAmount = container.clientWidth;
    container.scrollBy({
      left: direction*scrollAmount,
      behavior: 'smooth'
    })
  }

  openImage(url: string){
    this.selectedImage.set(url);
  }

  closeImage(){
    this.selectedImage.set(null);
  }

  plantDetails = computed(() => {
    const plant = this.plant();

    return [
      { 
        label: this.translate.instant('plantDetails.family'), 
        value: this.formatName(plant?.family), 
        icon: "assets/images/icons/plant.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.fragrance'), 
        value: this.formatName(plant?.fragrance), 
        icon: "assets/images/icons/flowers.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.hardiness'), 
        value: this.formatName(plant?.hardinessLevel), 
        icon: "assets/images/icons/snowflake.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.pollinator'), 
        value: plant?.isPlantForPollinators ? this.translate.instant('common.yes') : this.translate.instant('common.no'), 
        icon: "assets/images/icons/bee.svg" 
      }
    ]
  });

  plantSpecs = computed(() => {
    const plant = this.plant();

    return [
      { 
        label: this.translate.instant('plantDetails.drought'), 
        value: plant?.isDroughtResistant ? this.translate.instant('common.yes') : this.translate.instant('common.no'),
        icon: "assets/images/icons/drop.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.lowMaintenance'), 
        value: plant?.isLowMaintenance ? this.translate.instant('common.yes') : this.translate.instant('common.no'),
        icon: "assets/images/icons/clean.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.spread'), 
        value: this.formatName(plant?.spreadType), 
        icon: "assets/images/icons/left-and-right-arrows.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.height'), 
        value: this.formatName(plant?.heightType), 
        icon: "assets/images/icons/resize.svg" 
      },
      { 
        label: this.translate.instant('plantDetails.time'), 
        value: this.formatName(plant?.timeToFullHeight), 
        icon: "assets/images/icons/clock.svg" 
      }
    ]
  });

  optionsMapping = computed(() => {
    const plant = this.plant()
  
    return [
      { key: this.translate.instant('plantDetails.cultivation'), value: plant?.cultivation },
      { key: this.translate.instant('plantDetails.pest'), value: plant?.pestResistance },
      { key: this.translate.instant('plantDetails.disease'), value: plant?.diseaseResistance },
      { key: this.translate.instant('plantDetails.pruning'), value: plant?.pruning },
      { key: this.translate.instant('plantDetails.propagation'), value: plant?.propagation },
    ]
  });

  growthConditions = computed(() => {
    const plant = this.plant()
    return [
      { label: this.translate.instant('plantDetails.season'), value: this.formatList(plant?.seasons), icon: "assets/images/icons/season.svg" },
      { label: this.translate.instant('plantDetails.soil'), value: this.formatList(plant?.soilTypes), icon: "assets/images/icons/soil.svg" },
      { label: this.translate.instant('plantDetails.sunlight'), value: this.formatList(plant?.sunlights), icon: "assets/images/icons/sun.svg" },
      { label: this.translate.instant('plantDetails.aspect'), value: this.formatList(plant?.aspects), icon: "assets/images/icons/sunrise.svg" },
      { label: this.translate.instant('plantDetails.moisture'), value: this.formatList(plant?.moistures), icon: "assets/images/icons/moisture.svg" },
      { label: this.translate.instant('plantDetails.exposure'), value: this.formatList(plant?.exposures), icon: "assets/images/icons/wind.svg" },
      { label: this.translate.instant('plantDetails.habit'), value: this.formatList(plant?.habits), icon: "assets/images/icons/forest.svg" },
      { label: this.translate.instant('plantDetails.ph'), value: this.formatList(plant?.phs), icon: "assets/images/icons/ph-balance.svg" }
    ]
  })

  options = computed(() => {
    const p = this.plant();
    if (!p) return [];

    const strings: string[] = [];
    if (p.cultivation) strings.push(this.translate.instant('plantDetails.cultivation'));
    if (p.pestResistance) strings.push(this.translate.instant('plantDetails.pest'));
    if (p.diseaseResistance) strings.push(this.translate.instant('plantDetails.disease'));
    if (p.pruning) strings.push(this.translate.instant('plantDetails.pruning'));
    if (p.propagation) strings.push(this.translate.instant('plantDetails.propagation'));
    
    return strings;
  })

  optionText = computed(() => {
    const selected = this.selectedOption();
    const mapping = this.optionsMapping().find(m => m.key === selected);
    return mapping ? mapping.value : null;
  })

  clickSelectedOption(option: string){
    this.selectedOption.set(option);
  }

  formatList(items: Reference[] | undefined): string {
    if (!items || items.length === 0) return 'Not specified';
    return items.map(i => i.name).join(', ');
  }

  formatName(item: Reference | undefined): string {
    return item ? item.name : 'Not Specified';
  }

  toggleAddPlantedModal(){
    this.isAddPlantedModalOpen.update(val => !val);

    if (this.isAddPlantedModalOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  toggleOptionsMenu(){
    this.isOptionsMenuOpened.update(val => !val);
  }

  editPlant(){
    this.router.navigate(['/plant-form', this.id]);
    this.isOptionsMenuOpened.set(false);
  }

  deletePlant(){
    this.service.removePlant(parseInt(this.id)).subscribe({
      next: () => {
        this.router.navigate(['']);
        this.notif.showSuccess("Successfully deleted plant")
      },
      error: () => this.notif.showError("Couldn't remove plant, try again later!")
    });
    this.isOptionsMenuOpened.set(false);
  }
}
