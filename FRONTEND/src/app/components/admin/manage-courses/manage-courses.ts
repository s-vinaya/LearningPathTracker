import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MatIconModule } from '@angular/material/icon';
import { SidebarComponent } from '../sidebar/sidebar';
import { CourseService, Course } from '../../../services/course.service';
import { ToastService } from '../../../services/toast.service';
import { ConfirmService } from '../../../services/confirm.service';

@Pipe({ name: 'safe', standalone: true })
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  transform(url: string): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}

@Component({
  selector: 'app-manage-courses',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, SafePipe, MatIconModule],
  templateUrl: './manage-courses.html',
  styleUrl: './manage-courses.css',
})
export class ManageCourses implements OnInit {
  courses: Course[] = [];
  filteredCourses: Course[] = [];
  searchTerm: string = '';
  selectedStatus: string = 'all';
  selectedCategory: string = 'all';
  loading: boolean = true;

  constructor(private courseService: CourseService, private toast: ToastService, private confirm: ConfirmService) {}

  ngOnInit(): void {
    this.loadCourses();
    this.loadStatistics();
  }

  loadStatistics(): void {
    this.courseService.getCourseStatistics().subscribe({
      next: (stats) => {
        this.totalEnrollments = stats.totalEnrollments;
      },
      error: (err) => console.error('Error loading statistics:', err)
    });
  }

  loadCourses(): void {
    this.loading = true;
    this.courseService.getAllCourses().subscribe({
      next: (data) => {
        this.courses = data;
        this.filteredCourses = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading courses:', err);
        this.loading = false;
      }
    });
  }

  get totalCourses(): number {
    return this.courses.length;
  }

  get activeCourses(): number {
    return this.courses.filter(c => c.isActive).length;
  }

  get draftCourses(): number {
    return this.courses.filter(c => !c.isActive).length;
  }

  totalEnrollments: number = 0;
  showModal: boolean = false;
  showVideoModal: boolean = false;
  showBulkUploadModal: boolean = false;
  currentVideoId: string = '';
  bulkUploadResult: any = null;
  pendingVideos: any[] = [];
  fetchingBulk: boolean = false;
  editingCourse: Course | null = null;
  newCourse = {
    title: '',
    description: '',
    instructor: '',
    durationHours: 0,
    category: '',
    isActive: true,
    youTubeUrl: '',
    youTubeVideoId: '',
    thumbnailUrl: '',
    videoDuration: ''
  };
  fetchingYouTube: boolean = false;

  filterCourses(): void {
    this.filteredCourses = this.courses.filter(course => {
      const matchesSearch = course.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           course.description.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = this.selectedStatus === 'all' || 
                           (this.selectedStatus === 'active' && course.isActive) ||
                           (this.selectedStatus === 'draft' && !course.isActive);
      const matchesCategory = this.selectedCategory === 'all' || course.category === this.selectedCategory;
      return matchesSearch && matchesStatus && matchesCategory;
    });
  }

  deleteCourse(id: number): void {
    this.confirm.show('Are you sure you want to delete this course?', () => {
      this.courseService.deleteCourse(id).subscribe({
        next: () => {
          this.toast.show('Course deleted successfully', 'success');
          this.loadCourses();
        },
        error: (err) => {
          this.toast.show('Failed to delete course', 'error');
          console.error('Error deleting course:', err);
        }
      });
    });
  }

  getCourseImage(index: number): string {
    const images = [
      'https://images.unsplash.com/photo-1516116216624-53e697fedbea?w=400',
      'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400',
      'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400'
    ];
    return images[index % images.length];
  }

  formatDuration(minutes: number): string {
    const hrs = Math.floor(minutes / 60);
    const mins = minutes % 60;
    if (hrs > 0) {
      return `${hrs}h ${mins}m`;
    }
    return `${mins}m`;
  }

  playVideo(videoId?: string): void {
    if (videoId) {
      this.currentVideoId = videoId;
      this.showVideoModal = true;
    }
  }

  closeVideoModal(): void {
    this.showVideoModal = false;
    this.currentVideoId = '';
  }

  openModal(): void {
    this.editingCourse = null;
    this.showModal = true;
    this.resetForm();
  }

  editCourse(course: Course): void {
    this.editingCourse = course;
    this.newCourse = {
      title: course.title,
      description: course.description,
      instructor: course.instructor,
      durationHours: course.durationHours,
      category: course.category,
      isActive: course.isActive,
      youTubeUrl: course.youTubeUrl || '',
      youTubeVideoId: course.youTubeVideoId || '',
      thumbnailUrl: course.thumbnailUrl || '',
      videoDuration: course.videoDuration || ''
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.resetForm();
  }

  openBulkUploadModal(): void {
    this.showBulkUploadModal = true;
    this.bulkUploadResult = null;
  }

  closeBulkUploadModal(): void {
    this.showBulkUploadModal = false;
    this.bulkUploadResult = null;
    this.pendingVideos = [];
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file && file.type === 'text/plain') {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const content = e.target.result;
        const urls = content.split('\n').map((url: string) => url.trim()).filter((url: string) => url);
        this.fetchBulkVideos(urls);
      };
      reader.readAsText(file);
    } else {
      this.toast.show('Please select a valid .txt file', 'error');
    }
  }

  fetchBulkVideos(urls: string[]): void {
    this.fetchingBulk = true;
    this.pendingVideos = [];
    const existingUrls = this.courses.map(c => c.youTubeUrl);
    let processed = 0;

    urls.forEach(url => {
      if (existingUrls.includes(url)) {
        processed++;
        return;
      }

      this.courseService.getYouTubeVideoInfo(url).subscribe({
        next: (data) => {
          this.pendingVideos.push({
            youTubeUrl: url,
            youTubeVideoId: data.videoId,
            title: data.title,
            description: data.description,
            instructor: data.channelTitle,
            thumbnailUrl: data.thumbnailUrl,
            durationHours: parseInt(data.duration) || 0,
            category: '',
            isActive: true
          });
          processed++;
          if (processed === urls.length) this.fetchingBulk = false;
        },
        error: () => {
          processed++;
          if (processed === urls.length) this.fetchingBulk = false;
        }
      });
    });
  }

  uploadBulkCourses(): void {
    let uploaded = 0;
    this.pendingVideos.forEach(video => {
      this.courseService.createCourse(video).subscribe({
        next: () => {
          uploaded++;
          if (uploaded === this.pendingVideos.length) {
            this.bulkUploadResult = {
              totalProcessed: this.pendingVideos.length,
              successCount: uploaded,
              skippedCount: 0
            };
            this.pendingVideos = [];
            this.loadCourses();
          }
        },
        error: () => {
          uploaded++;
          if (uploaded === this.pendingVideos.length) {
            this.loadCourses();
          }
        }
      });
    });
  }

  removePendingVideo(index: number): void {
    this.pendingVideos.splice(index, 1);
  }

  resetForm(): void {
    this.newCourse = {
      title: '',
      description: '',
      instructor: '',
      durationHours: 0,
      category: '',
      isActive: true,
      youTubeUrl: '',
      youTubeVideoId: '',
      thumbnailUrl: '',
      videoDuration: ''
    };
  }

  onYouTubePaste(event: ClipboardEvent): void {
    const pastedText = event.clipboardData?.getData('text');
    console.log('Pasted text:', pastedText);
    setTimeout(() => {
      console.log('Current URL value:', this.newCourse.youTubeUrl);
      if (this.newCourse.youTubeUrl && (this.newCourse.youTubeUrl.includes('youtube') || this.newCourse.youTubeUrl.includes('youtu.be'))) {
        this.fetchYouTubeData();
      } else {
        console.log('URL does not contain youtube');
      }
    }, 200);
  }

  onYouTubeUrlChange(): void {
    console.log('URL changed:', this.newCourse.youTubeUrl);
    if (this.newCourse.youTubeUrl && (this.newCourse.youTubeUrl.includes('youtube') || this.newCourse.youTubeUrl.includes('youtu.be')) && !this.fetchingYouTube) {
      console.log('Triggering fetch from input change');
      this.fetchYouTubeData();
    }
  }

  fetchYouTubeData(): void {
    console.log('Fetching YouTube data for:', this.newCourse.youTubeUrl);
    this.fetchingYouTube = true;
    this.courseService.getYouTubeVideoInfo(this.newCourse.youTubeUrl).subscribe({
      next: (data) => {
        console.log('YouTube data received:', data);
        this.newCourse.title = data.title;
        this.newCourse.description = data.description;
        this.newCourse.instructor = data.channelTitle;
        this.newCourse.youTubeVideoId = data.videoId;
        this.newCourse.thumbnailUrl = data.thumbnailUrl;
        this.newCourse.durationHours = parseInt(data.duration) || 0;
        this.fetchingYouTube = false;
      },
      error: (err) => {
        console.error('Error fetching YouTube data:', err);
        console.error('Error details:', err.error);
        this.fetchingYouTube = false;
        this.toast.show('Failed to fetch YouTube video details: ' + (err.error?.message || err.message), 'error');
      }
    });
  }

  saveCourse(): void {
    if (!this.newCourse.title || !this.newCourse.instructor || !this.newCourse.category) {
      this.toast.show('Please fill in required fields', 'error');
      return;
    }

    if (this.editingCourse) {
      this.courseService.updateCourse(this.editingCourse.id, this.newCourse).subscribe({
        next: () => {
          this.closeModal();
          this.loadCourses();
          this.loadStatistics();
        },
        error: (err) => {
          console.error('Error updating course:', err);
          this.toast.show('Failed to update course', 'error');
        }
      });
    } else {
      this.courseService.createCourse(this.newCourse).subscribe({
        next: () => {
          this.closeModal();
          this.loadCourses();
          this.loadStatistics();
        },
        error: (err) => {
          console.error('Error creating course:', err);
          this.toast.show('Failed to create course', 'error');
        }
      });
    }
  }
}
