import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantedGrowthLog } from './planted-growth-log';

describe('PlantedGrowthLog', () => {
  let component: PlantedGrowthLog;
  let fixture: ComponentFixture<PlantedGrowthLog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantedGrowthLog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantedGrowthLog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
