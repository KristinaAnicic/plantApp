import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PlantedService } from '../../services/planted.service';
import { PlantedDto } from '../../models/planted.interface';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-plant-graveyard',
  imports: [RouterLink, TranslateModule],
  templateUrl: './plant-graveyard.html',
  styleUrl: './plant-graveyard.css',
})
export class PlantGraveyard {
  private plantedService = inject(PlantedService);
  plants = signal<PlantedDto[] | null>(null);

  ngOnInit(): void {
    this.loadPlace();
  }

  loadPlace(){
    this.plantedService.getAllDeadPlants().subscribe({
      next: (response) => {
        this.plants.set(response);
      },
      error: (err:any) => {
        this.plants.set(null);
        console.log("Error while fetching plants from graveyard", err);
      }
    });
  }
}
