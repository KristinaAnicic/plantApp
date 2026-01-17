import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPlants } from './user-plants';

describe('UserPlants', () => {
  let component: UserPlants;
  let fixture: ComponentFixture<UserPlants>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserPlants]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserPlants);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
