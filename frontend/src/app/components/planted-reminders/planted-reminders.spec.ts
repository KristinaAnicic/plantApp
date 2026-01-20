import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantedReminders } from './planted-reminders';

describe('PlantedReminders', () => {
  let component: PlantedReminders;
  let fixture: ComponentFixture<PlantedReminders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlantedReminders]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlantedReminders);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
