import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantService } from '../../services/plant.service';
import { PlantGetDto } from '../../models/plant.interface';

@Component({
  selector: 'app-plant',
  imports: [],
  templateUrl: './plant.html',
  styleUrl: './plant.css',
})
export class Plant implements OnInit {
  @Input() id!: string;
  
  private service = inject(PlantService);
  plant = signal<PlantGetDto | null>(null);
  selectedImage = signal<string | null>(null);
  selectedOption = signal<string | null>(null);

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

  plantDetails = computed(() => [
    { label: 'Family', value: this.plant()?.family, icon: "assets/images/icons/plant.svg" },
    { label: 'Fragrance', value: this.plant()?.fragrance, icon: "assets/images/icons/flowers.svg" },
    { label: 'Hardiness', value: this.plant()?.hardinessLevel, icon: "assets/images/icons/snowflake.svg" },
    { label: 'Pollinator Friendly', value: this.plant()?.isPlantForPollinators ? 'Yes' : 'No', icon: "assets/images/icons/bee.svg" }
  ]);

  plantSpecs = computed(() => [
    { label: 'Drought Resistant', value: this.plant()?.isDroughtResistant ? 'Yes' : 'No', icon: "assets/images/icons/drop.svg" },
    { label: 'Low Maintenace', value: this.plant()?.isLowMaintenance ? 'Yes' : 'No', icon: "assets/images/icons/clean.svg" },
    { label: 'Spread', value: this.plant()?.spreadType, icon: "assets/images/icons/left-and-right-arrows.svg" },
    { label: 'Height', value: this.plant()?.heightType, icon: "assets/images/icons/resize.svg" },
    { label: 'Time to full height', value: this.plant()?.timeToFullHeight, icon: "assets/images/icons/clock.svg" }
  ]);

  optionsMapping = computed(() => [
    { key: "Cultivation", value: this.plant()?.cultivation },
    { key: "Pest resistance", value: this.plant()?.pestResistance },
    { key: "Disease resistance", value: this.plant()?.diseaseResistance },
    { key: "Pruning", value: this.plant()?.pruning },
    { key: "Propagation", value: this.plant()?.propagation },
  ]);

  growthConditions = computed(() => [
    { label: 'Seasons', value: this.plant()?.seasons?.join(", "), icon: "assets/images/icons/season.svg" },
    { label: 'Soil Types', value: this.plant()?.soilTypes, icon: "assets/images/icons/soil.svg" },
    { label: 'Sunlight', value: this.plant()?.sunlights, icon: "assets/images/icons/sun.svg" },
    { label: 'Aspects', value: this.plant()?.aspects, icon: "assets/images/icons/sunrise.svg" },
    { label: 'Moisture', value: this.plant()?.moistures, icon: "assets/images/icons/moisture.svg" },
    { label: 'Exposure', value: this.plant()?.exposures, icon: "assets/images/icons/wind.svg" },
    { label: 'Habits', value: this.plant()?.habits?.join(", "), icon: "assets/images/icons/forest.svg" },
    { label: 'Ph', value: this.plant()?.phs, icon: "assets/images/icons/ph-balance.svg" }
  ])

  options = computed(() => {
    const p = this.plant();
    if (!p) return [];

    const strings: string[] = [];
    if (p.cultivation) strings.push("Cultivation");
    if (p.pestResistance) strings.push("Pest resistance");
    if (p.diseaseResistance) strings.push("Disease resistance");
    if (p.pruning) strings.push("Pruning");
    if (p.propagation) strings.push("Propagation");
    
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
}
