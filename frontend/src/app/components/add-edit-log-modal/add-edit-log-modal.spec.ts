import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditLogModal } from './add-edit-log-modal';

describe('AddEditLogModal', () => {
  let component: AddEditLogModal;
  let fixture: ComponentFixture<AddEditLogModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditLogModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditLogModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
