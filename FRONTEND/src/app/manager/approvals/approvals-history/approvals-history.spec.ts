import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalsHistory } from './approvals-history';

describe('ApprovalsHistory', () => {
  let component: ApprovalsHistory;
  let fixture: ComponentFixture<ApprovalsHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApprovalsHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApprovalsHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
