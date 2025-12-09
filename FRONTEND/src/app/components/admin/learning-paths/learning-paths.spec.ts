import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningPaths } from './learning-paths';

describe('LearningPaths', () => {
  let component: LearningPaths;
  let fixture: ComponentFixture<LearningPaths>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LearningPaths]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningPaths);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
