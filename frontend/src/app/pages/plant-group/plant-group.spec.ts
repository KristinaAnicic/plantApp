import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantGroup } from './plant-group';

describe('PlantGroup', () => {
  let component: PlantGroup;
  let fixture: ComponentFixture<PlantGroup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantGroup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantGroup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
