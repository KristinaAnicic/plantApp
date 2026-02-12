import { TestBed } from '@angular/core/testing';

import { PlantGroupService } from './plant-group.service';

describe('PlantGroupService', () => {
  let service: PlantGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlantGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
