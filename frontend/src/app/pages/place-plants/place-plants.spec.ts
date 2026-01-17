import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlacePlants } from './place-plants';

describe('PlacePlants', () => {
  let component: PlacePlants;
  let fixture: ComponentFixture<PlacePlants>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlacePlants]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlacePlants);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
