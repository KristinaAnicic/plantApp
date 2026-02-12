import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantGroupList } from './plant-group-list';

describe('PlantGroupList', () => {
  let component: PlantGroupList;
  let fixture: ComponentFixture<PlantGroupList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantGroupList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantGroupList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
