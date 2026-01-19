import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantService } from '../../services/plant.service';
import { PlantGetDto } from '../../models/plant.interface';
import { Reference } from '../../models/reference.interface';

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

  plantDetails = computed(() => {
    const plant = this.plant();

    return [
      { label: 'Family', value: this.formatName(plant?.family), icon: "assets/images/icons/plant.svg" },
      { label: 'Fragrance', value: this.formatName(plant?.fragrance), icon: "assets/images/icons/flowers.svg" },
      { label: 'Hardiness', value: this.formatName(plant?.hardinessLevel), icon: "assets/images/icons/snowflake.svg" },
      { label: 'Pollinator Friendly', value: plant?.isPlantForPollinators ? 'Yes' : 'No', icon: "assets/images/icons/bee.svg" }
    ]
  });

  plantSpecs = computed(() => {
    const plant = this.plant();

    return [
      { label: 'Drought Resistant', value: plant?.isDroughtResistant ? 'Yes' : 'No', icon: "assets/images/icons/drop.svg" },
      { label: 'Low Maintenace', value: plant?.isLowMaintenance ? 'Yes' : 'No', icon: "assets/images/icons/clean.svg" },
      { label: 'Spread', value: this.formatName(plant?.spreadType), icon: "assets/images/icons/left-and-right-arrows.svg" },
      { label: 'Height', value: this.formatName(plant?.heightType), icon: "assets/images/icons/resize.svg" },
      { label: 'Time to full height', value: this.formatName(plant?.timeToFullHeight), icon: "assets/images/icons/clock.svg" }
    ]
  });

  optionsMapping = computed(() => {
    const plant = this.plant()
  
    return [
      { key: "Cultivation", value: plant?.cultivation },
      { key: "Pest resistance", value: plant?.pestResistance },
      { key: "Disease resistance", value: plant?.diseaseResistance },
      { key: "Pruning", value: plant?.pruning },
      { key: "Propagation", value: plant?.propagation },
    ]
  });

  growthConditions = computed(() => {
    const plant = this.plant()
    return [
      { label: 'Season', value: this.formatList(plant?.seasons), icon: "assets/images/icons/season.svg" },
      { label: 'Soil Type', value: this.formatList(plant?.soilTypes), icon: "assets/images/icons/soil.svg" },
      { label: 'Sunlight', value: this.formatList(plant?.sunlights), icon: "assets/images/icons/sun.svg" },
      { label: 'Aspect', value: this.formatList(plant?.aspects), icon: "assets/images/icons/sunrise.svg" },
      { label: 'Moisture', value: this.formatList(plant?.moistures), icon: "assets/images/icons/moisture.svg" },
      { label: 'Exposure', value: this.formatList(plant?.exposures), icon: "assets/images/icons/wind.svg" },
      { label: 'Habit', value: this.formatList(plant?.habits), icon: "assets/images/icons/forest.svg" },
      { label: 'Ph', value: this.formatList(plant?.phs), icon: "assets/images/icons/ph-balance.svg" }
    ]
  })

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

  formatList(items: Reference[] | undefined): string {
    if (!items || items.length === 0) return 'Not specified';
    return items.map(i => i.name).join(', ');
  }

  formatName(item: Reference | undefined): string {
    return item ? item.name : 'Not Specified';
  }
}
