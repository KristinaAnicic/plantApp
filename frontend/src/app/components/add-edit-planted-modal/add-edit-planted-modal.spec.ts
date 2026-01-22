import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditPlantedModal } from './add-edit-planted-modal';

describe('AddEditPlantedModal', () => {
  let component: AddEditPlantedModal;
  let fixture: ComponentFixture<AddEditPlantedModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditPlantedModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditPlantedModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
