import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantExchangeList } from './plant-exchange-list';

describe('PlantExchangeList', () => {
  let component: PlantExchangeList;
  let fixture: ComponentFixture<PlantExchangeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantExchangeList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantExchangeList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
