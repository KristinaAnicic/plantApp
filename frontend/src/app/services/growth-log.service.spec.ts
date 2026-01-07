import { TestBed } from '@angular/core/testing';

import { GrowthLogService } from './growth-log.service';

describe('GrowthLogService', () => {
  let service: GrowthLogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GrowthLogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
