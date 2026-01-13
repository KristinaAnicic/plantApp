import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Plant } from './plant';

describe('Plant', () => {
  let component: Plant;
  let fixture: ComponentFixture<Plant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Plant]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Plant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
