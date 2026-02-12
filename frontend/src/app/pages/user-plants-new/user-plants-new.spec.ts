import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPlantsNew } from './user-plants-new';

describe('UserPlantsNew', () => {
  let component: UserPlantsNew;
  let fixture: ComponentFixture<UserPlantsNew>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserPlantsNew]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserPlantsNew);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
