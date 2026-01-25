import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditExchange } from './add-edit-exchange';

describe('AddEditExchange', () => {
  let component: AddEditExchange;
  let fixture: ComponentFixture<AddEditExchange>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditExchange]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditExchange);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
