import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalDetailsDialog } from './approval-details-dialog';

describe('ApprovalDetailsDialog', () => {
  let component: ApprovalDetailsDialog;
  let fixture: ComponentFixture<ApprovalDetailsDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApprovalDetailsDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApprovalDetailsDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
