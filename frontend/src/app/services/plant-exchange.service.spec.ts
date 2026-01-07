import { TestBed } from '@angular/core/testing';

import { PlantExchangeService } from './plant-exchange.service';

describe('PlantExchangeService', () => {
  let service: PlantExchangeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlantExchangeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
