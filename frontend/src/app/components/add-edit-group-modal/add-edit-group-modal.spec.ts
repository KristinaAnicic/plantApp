import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditGroupModal } from './add-edit-group-modal';

describe('AddEditGroupModal', () => {
  let component: AddEditGroupModal;
  let fixture: ComponentFixture<AddEditGroupModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditGroupModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditGroupModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
