import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseRequest } from './course-request';

describe('CourseRequest', () => {
  let component: CourseRequest;
  let fixture: ComponentFixture<CourseRequest>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseRequest]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseRequest);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
