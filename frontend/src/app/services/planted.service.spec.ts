import { TestBed } from '@angular/core/testing';

import { PlantedService } from './planted.service';

describe('PlantedService', () => {
  let service: PlantedService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlantedService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
