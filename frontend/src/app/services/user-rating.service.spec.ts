import { TestBed } from '@angular/core/testing';

import { UserRatingService } from './user-rating.service';

describe('UserRatingService', () => {
  let service: UserRatingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserRatingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
