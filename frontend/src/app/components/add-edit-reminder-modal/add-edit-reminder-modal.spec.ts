import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditReminderModal } from './add-edit-reminder-modal';

describe('AddEditReminderModal', () => {
  let component: AddEditReminderModal;
  let fixture: ComponentFixture<AddEditReminderModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditReminderModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditReminderModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
