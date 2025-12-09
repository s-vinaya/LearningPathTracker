import { TestBed } from '@angular/core/testing';

import { ManagerApproval } from './manager-approval';

describe('ManagerApproval', () => {
  let service: ManagerApproval;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManagerApproval);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
