import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantExchange } from './plant-exchange';

describe('PlantExchange', () => {
  let component: PlantExchange;
  let fixture: ComponentFixture<PlantExchange>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantExchange]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantExchange);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
