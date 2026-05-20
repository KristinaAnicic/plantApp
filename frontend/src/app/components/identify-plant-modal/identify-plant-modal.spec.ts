import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IdentifyPlantModal } from './identify-plant-modal';

describe('IdentifyPlantModal', () => {
  let component: IdentifyPlantModal;
  let fixture: ComponentFixture<IdentifyPlantModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IdentifyPlantModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IdentifyPlantModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
