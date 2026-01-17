import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUpdatePlaceModal } from './add-update-place-modal';

describe('AddUpdatePlaceModal', () => {
  let component: AddUpdatePlaceModal;
  let fixture: ComponentFixture<AddUpdatePlaceModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddUpdatePlaceModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddUpdatePlaceModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
