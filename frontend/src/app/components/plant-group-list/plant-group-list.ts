import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { PlantedService } from '../../services/planted.service';
import { PlantedDto, PlantedWithAnyDeadBoolDto } from '../../models/planted.interface';
import { PlantGroupService } from '../../services/plant-group.service';
import { NotificationService } from '../../services/notification.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-plant-group-list',
  imports: [TranslateModule],
  templateUrl: './plant-group-list.html',
  styleUrl: './plant-group-list.css',
})
export class PlantGroupList implements OnInit {
  close = output<void>();
  addedPlants = output<void>();
  groupId = input.required<number>();

  plantedService = inject(PlantedService);
  groupService = inject(PlantGroupService);
  private notificationService = inject(NotificationService);
  

  plantedList = signal<PlantedWithAnyDeadBoolDto | null>(null);
  selectedPlants = signal<number[]>([]);
  
  ngOnInit(): void {
    this.loadPlanted();
  }

  loadPlanted(){
    this.plantedService.getAllPlantedPlants().subscribe({
      next: (result) => {
        this.plantedList.set(result);

        if (this.groupId != null) {
          const preselected = result.planted
            .filter(plant => plant.plantGroup?.id === this.groupId())
            .map(plant => plant.id);

          this.selectedPlants.set(preselected);
        }
      },
      error: (err) => {
        console.log("Error while fetching user plant: ", err);
      }
    })
  }

  onCloseClick(){
    this.close.emit();
  }

  togglePlantSelection(plantId: number) {
    const current = [...this.selectedPlants()];
    const index = current.indexOf(plantId);

    if (index > -1) {
      current.splice(index, 1);
    } else {
      current.push(plantId);
    }

    this.selectedPlants.set(current);
  }

  saveList(){
    this.groupService.setMultiplePlantsToGroup(this.groupId(), this.selectedPlants()).subscribe({
      next:() => {
        this.notificationService.showSuccess('Successfully added plants');
        this.addedPlants.emit();
      },
      error:(err:any) => {
        this.notificationService.showError(err.error.error);
        console.log("Error on adding planted: ", err);
      }
    })
  }
}
