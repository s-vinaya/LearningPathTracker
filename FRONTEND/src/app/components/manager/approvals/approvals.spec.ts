import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagerApprovalsComponent } from './approvals';

describe('ManagerApprovalsComponent', () => {
  let component: ManagerApprovalsComponent;
  let fixture: ComponentFixture<ManagerApprovalsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagerApprovalsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagerApprovalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});