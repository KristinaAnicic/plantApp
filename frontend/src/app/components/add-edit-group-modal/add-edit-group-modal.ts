import { Component, computed, inject, input, output, signal } from '@angular/core';
import { UpsertPlantGroupDto } from '../../models/plant-group.interface';
import { PlantGroupService } from '../../services/plant-group.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-edit-group-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './add-edit-group-modal.html',
  styleUrl: './add-edit-group-modal.css',
})
export class AddEditGroupModal {
  close = output<void>();
  groupAdded = output<void>();
  editGroup = input<UpsertPlantGroupDto | null>(null);

  plantGroupService = inject(PlantGroupService);
  showWarning = signal(false);
  errorMessage = signal('');

  isEditing = computed(() => !!this.editGroup());
  headerText = computed(() => this.isEditing() ? 'Edit group' : 'Add a new group');
  buttonText = computed(() => this.isEditing() ? 'Save changes' : 'Add group');

  groupForm = new FormGroup({
    id: new FormControl(0, { nonNullable: true }),
    name: new FormControl('', { nonNullable: true }),
    description: new FormControl('', { nonNullable: true }),
  });

  ngOnInit(): void {
    const group = this.editGroup();

    if (group) {
      this.groupForm.patchValue(group);
    }
  }

  onCloseClick(){
    this.close.emit();
  }

  closeWarningClick(){
    this.showWarning.set(false);
  }

  addEditGroup(){
    const data: UpsertPlantGroupDto = this.groupForm.getRawValue();
    const cleanData: UpsertPlantGroupDto = {
      name: data.name.trim(),
      description: data.description?.trim(),
    };

    if (this.isEditing()){
      const id = this.editGroup()?.id;

      if (id === undefined) {
        console.error("Cannot save changes for group without providing id.");
        return;
      }

      this.plantGroupService.updateGroup(id, cleanData).subscribe({
        next:() => {
          this.groupAdded.emit();
          this.onCloseClick();
        },
        error:(err:any) => {
          this.showWarning.set(true);
          this.errorMessage.set(err.error.error);
          console.log("Error on editing group", err);
        }
      })
    }
    
    else {
      this.plantGroupService.addGroup(cleanData).subscribe({
        next:() => {
          this.groupAdded.emit();
          this.onCloseClick();
        },
        error:(err:any) => {
          this.showWarning.set(true);
          this.errorMessage.set(err.error.error);
          console.log("Error on adding a place", err);
        }
      })
    }
    
  }
}
