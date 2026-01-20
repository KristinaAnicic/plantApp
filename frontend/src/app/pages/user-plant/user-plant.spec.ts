import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPlant } from './user-plant';

describe('UserPlant', () => {
  let component: UserPlant;
  let fixture: ComponentFixture<UserPlant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserPlant]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserPlant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
