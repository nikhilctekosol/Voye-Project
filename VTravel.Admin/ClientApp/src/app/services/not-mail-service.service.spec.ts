import { TestBed } from '@angular/core/testing';

import { NotMailServiceService } from './not-mail-service.service';

describe('NotMailServiceService', () => {
  let service: NotMailServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotMailServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
