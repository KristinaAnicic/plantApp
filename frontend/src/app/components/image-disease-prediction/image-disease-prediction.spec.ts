import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageDiseasePrediction } from './image-disease-prediction';

describe('ImageDiseasePrediction', () => {
  let component: ImageDiseasePrediction;
  let fixture: ComponentFixture<ImageDiseasePrediction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImageDiseasePrediction]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImageDiseasePrediction);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
