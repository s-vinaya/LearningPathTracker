import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageQuizzes } from './manage-quizzes';

describe('ManageQuizzes', () => {
  let component: ManageQuizzes;
  let fixture: ComponentFixture<ManageQuizzes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageQuizzes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageQuizzes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
