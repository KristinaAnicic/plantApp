import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantGraveyard } from './plant-graveyard';

describe('PlantGraveyard', () => {
  let component: PlantGraveyard;
  let fixture: ComponentFixture<PlantGraveyard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantGraveyard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantGraveyard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
