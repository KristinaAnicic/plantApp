import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditPlant } from './add-edit-plant';

describe('AddEditPlant', () => {
  let component: AddEditPlant;
  let fixture: ComponentFixture<AddEditPlant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditPlant]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditPlant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
