import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditReviewModal } from './add-edit-review-modal';

describe('AddEditReviewModal', () => {
  let component: AddEditReviewModal;
  let fixture: ComponentFixture<AddEditReviewModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditReviewModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditReviewModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
