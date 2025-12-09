import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ProfileComponent } from './profile';
import { ProfileService } from '../../../services/profile';
import { of, throwError } from 'rxjs';

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let profileService: jasmine.SpyObj<ProfileService>;

  beforeEach(async () => {
    const profileServiceSpy = jasmine.createSpyObj('ProfileService', ['getProfile', 'updateProfile', 'uploadProfileImage']);

    await TestBed.configureTestingModule({
      imports: [ProfileComponent, HttpClientTestingModule],
      providers: [
        { provide: ProfileService, useValue: profileServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    profileService = TestBed.inject(ProfileService) as jasmine.SpyObj<ProfileService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load profile on init', () => {
    const mockProfile = {
      id: 1,
      fullName: 'Test User',
      email: 'test@example.com',
      role: 'Admin',
      phoneNumber: '1234567890',
      department: 'IT',
      jobTitle: 'Developer'
    };

    profileService.getProfile.and.returnValue(of(mockProfile));
    component.ngOnInit();

    expect(profileService.getProfile).toHaveBeenCalled();
    expect(component.profile).toEqual(mockProfile);
  });

  it('should toggle edit mode', () => {
    expect(component.isEditing).toBeFalse();
    component.toggleEdit();
    expect(component.isEditing).toBeTrue();
    component.toggleEdit();
    expect(component.isEditing).toBeFalse();
  });

  it('should update profile', () => {
    component.profile = {
      id: 1,
      fullName: 'Updated Name',
      email: 'test@example.com',
      role: 'Admin',
      phoneNumber: '1234567890',
      department: 'IT',
      jobTitle: 'Developer'
    };

    profileService.updateProfile.and.returnValue(of({ message: 'Success' }));
    component.saveProfile();

    expect(profileService.updateProfile).toHaveBeenCalled();
    expect(component.isEditing).toBeFalse();
  });

  it('should handle file selection', () => {
    const file = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [file] } };

    component.onFileSelected(event);
    expect(component.selectedFile).toBe(file);
  });

  it('should upload profile image', () => {
    const file = new File([''], 'test.jpg', { type: 'image/jpeg' });
    component.selectedFile = file;

    profileService.uploadProfileImage.and.returnValue(of({ message: 'Success' }));
    profileService.getProfile.and.returnValue(of({
      id: 1,
      fullName: 'Test',
      email: 'test@example.com',
      role: 'Admin'
    }));

    component.uploadImage();
    expect(profileService.uploadProfileImage).toHaveBeenCalledWith(file);
  });
});
