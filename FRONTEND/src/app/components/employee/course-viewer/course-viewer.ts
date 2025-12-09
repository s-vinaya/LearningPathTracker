import { Component, OnInit, OnDestroy, AfterViewInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

declare var YT: any;

@Component({
  selector: 'app-course-viewer',
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './course-viewer.html',
  styleUrl: './course-viewer.css',
})
export class CourseViewer implements OnInit, OnDestroy, AfterViewInit {
  courseId!: number;
  course: any = null;
  loading = true;
  videoId: string = '';
  player: any;
  progressInterval: any;
  currentTime = 0;
  duration = 0;
  progressPercent = 0;
  playerReady = false;
  useIframe = false;
  iframeUrl: SafeResourceUrl | null = null;
  maxWatchedTime = 0;
  showSkipWarning = false;
  learningPathId: number | null = null;
  allCourses: any[] = [];

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private employeeService: EmployeeService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit() {
    this.courseId = +this.route.snapshot.params['id'];
    this.learningPathId = this.route.snapshot.queryParams['pathId'] ? +this.route.snapshot.queryParams['pathId'] : null;
    this.loadCourseDetails();
    this.loadYouTubeAPI();
  }

  ngAfterViewInit() {}

  ngOnDestroy() {
    if (this.progressInterval) {
      clearInterval(this.progressInterval);
    }
  }

  loadCourseDetails() {
    this.employeeService.getEnrolledCourses().subscribe({
      next: (courses: any[]) => {
        this.course = courses.find(c => c.id === this.courseId);
        if (this.course) {
          this.progressPercent = this.course.progress || 0;
          if (this.course.youTubeUrl) {
            this.videoId = this.extractVideoId(this.course.youTubeUrl) || '';
            this.duration = this.course.durationHours * 60;
            this.maxWatchedTime = Math.floor((this.progressPercent / 100) * this.duration);
            
            if (this.playerReady && this.videoId) {
              this.initPlayer();
            }
          }
          this.loading = false;
        } else {
          this.loadCourseFromAPI();
        }
      },
      error: (err: any) => {
        console.error('Error loading course:', err);
        this.loadCourseFromAPI();
      }
    });
  }

  loadCourseFromAPI() {
    this.employeeService.getAllAvailableLearningPaths().subscribe({
      next: (paths: any) => {
        let foundCourse = null;
        for (const path of paths) {
          if (path.courses) {
            foundCourse = path.courses.find((c: any) => {
              const courseId = c.courseId || c.id;
              return courseId === this.courseId;
            });
            if (foundCourse) break;
          }
        }
        
        if (foundCourse) {
          console.log('Found course:', foundCourse);
          const youtubeUrl = foundCourse.youTubeUrl || foundCourse.videoUrl || foundCourse.youtubeUrl || foundCourse.url;
          console.log('YouTube URL:', youtubeUrl);
          
          this.course = {
            id: foundCourse.id,
            title: foundCourse.title,
            description: foundCourse.description,
            instructor: foundCourse.instructor || 'Unknown',
            durationHours: foundCourse.durationHours || 0,
            youTubeUrl: youtubeUrl,
            progress: 0
          };
          this.progressPercent = 0;
          if (this.course.youTubeUrl) {
            this.videoId = this.extractVideoId(this.course.youTubeUrl) || '';
            console.log('Extracted video ID:', this.videoId);
            this.duration = this.course.durationHours * 60;
            this.maxWatchedTime = 0;
            
            if (this.playerReady && this.videoId) {
              this.initPlayer();
            }
          } else {
            console.error('No YouTube URL found in course data');
          }
          this.loading = false;
        } else {
          console.error('Course not found in learning paths');
          this.loading = false;
        }
      },
      error: (err: any) => {
        console.error('Error loading course from learning paths:', err);
        this.loading = false;
      }
    });
  }

  loadYouTubeAPI() {
    if (typeof YT === 'undefined' || typeof YT.Player === 'undefined') {
      const tag = document.createElement('script');
      tag.src = 'https://www.youtube.com/iframe_api';
      tag.onerror = () => {
        console.error('Failed to load YouTube API');
        this.playerReady = false;
      };
      document.body.appendChild(tag);
      (window as any).onYouTubeIframeAPIReady = () => {
        this.playerReady = true;
        if (this.videoId) this.initPlayer();
      };
    } else {
      this.playerReady = true;
      if (this.videoId) this.initPlayer();
    }
  }

  initPlayer() {
    if (typeof YT === 'undefined' || !YT.Player) {
      console.error('YouTube API not loaded');
      return;
    }
    
    const startSeconds = this.duration > 0 ? Math.floor((this.progressPercent / 100) * this.duration) : 0;
    
    this.player = new YT.Player('youtube-player', {
      height: '100%',
      width: '100%',
      videoId: this.videoId,
      playerVars: {
        autoplay: 0,
        controls: 1,
        modestbranding: 1,
        rel: 0,
        start: startSeconds
      },
      events: {
        onReady: (event: any) => this.onPlayerReady(event),
        onStateChange: (event: any) => this.onPlayerStateChange(event)
      }
    });
  }

  onPlayerReady(event: any) {
    this.duration = event.target.getDuration();
    this.maxWatchedTime = Math.floor((this.progressPercent / 100) * this.duration);
    const startSeconds = this.maxWatchedTime;
    if (startSeconds > 0) {
      event.target.seekTo(startSeconds, true);
    }
    this.startProgressTracking();
  }

  onPlayerStateChange(event: any) {
    if (event.data === YT.PlayerState.PLAYING) {
      this.startProgressTracking();
    } else if (event.data === YT.PlayerState.PAUSED || event.data === YT.PlayerState.ENDED) {
      if (this.progressInterval) {
        clearInterval(this.progressInterval);
      }
      this.updateProgress();
    }
  }

  extractVideoId(url: string): string | null {
    const patterns = [
      /(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&\n?#]+)/,
      /youtube\.com\/embed\/([^&\n?#]+)/
    ];
    
    for (const pattern of patterns) {
      const match = url.match(pattern);
      if (match && match[1]) {
        return match[1];
      }
    }
    return null;
  }

  startProgressTracking() {
    if (this.progressInterval) clearInterval(this.progressInterval);
    
    this.ngZone.runOutsideAngular(() => {
      this.progressInterval = setInterval(() => {
        if (this.player && this.player.getCurrentTime) {
          const newTime = Math.floor(this.player.getCurrentTime());
          const newDuration = Math.floor(this.player.getDuration());
          
          if (newDuration > 0) {
            // Check if user is trying to skip ahead
            if (newTime > this.maxWatchedTime + 2) {
              this.ngZone.run(() => {
                this.showSkipWarning = true;
                setTimeout(() => this.showSkipWarning = false, 3000);
              });
              this.player.seekTo(this.maxWatchedTime, true);
              return;
            }
            
            // Update max watched time
            if (newTime > this.maxWatchedTime) {
              this.maxWatchedTime = newTime;
            }
            
            const newProgress = Math.floor((this.maxWatchedTime / newDuration) * 100);
            
            this.ngZone.run(() => {
              this.currentTime = newTime;
              this.duration = newDuration;
              this.progressPercent = newProgress;
            });
            
            if (newTime % 5 === 0) {
              this.updateProgress();
            }
          }
        }
      }, 1000);
    });
  }

  updateProgress() {
    this.employeeService.updateVideoProgress(this.courseId, this.progressPercent).subscribe({
      next: () => {},
      error: (err: any) => console.error('Error updating progress:', err)
    });
  }

  startSimpleProgressTracking() {
    // Simple tracking doesn't auto-increment - user must watch
    // Progress is only updated when they interact with the video
  }

  get progressBarWidth(): string {
    return `${Math.min(this.progressPercent, 100)}%`;
  }

  get formattedTime(): string {
    const mins = Math.floor(this.currentTime / 60);
    const secs = this.currentTime % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }

  get formattedDuration(): string {
    const mins = Math.floor(this.duration / 60);
    const secs = this.duration % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }

  goBack() {
    if (this.learningPathId) {
      this.router.navigate(['/employee/modules', this.learningPathId]);
    } else {
      this.router.navigate(['/employee/courses']);
    }
  }

  handleNext() {
    console.log('=== handleNext called ===');
    console.log('Current course:', this.course);
    console.log('Current courseId:', this.courseId);
    console.log('Learning Path ID:', this.learningPathId);
    console.log('Has Quiz:', this.course?.hasQuiz);
    
    // Check if CURRENT course has a quiz
    if (this.course?.hasQuiz) {
      console.log('Current course has quiz - navigating to quiz');
      this.router.navigate(['/employee/quiz', this.courseId]);
    } else if (this.learningPathId) {
      console.log('Current course has no quiz - finding next course');
      // Current course has no quiz - navigate to next course in learning path
      this.employeeService.getAllAvailableLearningPaths().subscribe({
        next: (paths: any) => {
          console.log('Fetched paths:', paths);
          const path = paths.find((p: any) => p.id === this.learningPathId);
          console.log('Found learning path:', path);
          
          if (path?.courses) {
            console.log('Courses in path:', path.courses);
            const currentIndex = path.courses.findIndex((c: any) => {
              const courseId = c.courseId || c.id;
              console.log('Checking course:', courseId, 'against current:', this.courseId);
              return courseId === this.courseId;
            });
            console.log('Current course index:', currentIndex);
            
            if (currentIndex >= 0 && currentIndex < path.courses.length - 1) {
              const nextCourse = path.courses[currentIndex + 1];
              const nextCourseId = nextCourse.courseId || nextCourse.id;
              console.log('Next course:', nextCourse);
              console.log('Navigating to next course ID:', nextCourseId);
              
              this.ngZone.run(() => {
                this.router.navigate(['/employee/course-viewer', nextCourseId], { 
                  queryParams: { pathId: this.learningPathId } 
                }).then(success => {
                  console.log('Navigation result:', success);
                  if (success) {
                    window.location.reload();
                  }
                });
              });
            } else {
              console.log('No more courses in learning path');
              alert('Congratulations! You have completed all courses in this learning path.');
              this.ngZone.run(() => {
                this.router.navigate(['/employee/learning-paths']);
              });
            }
          } else {
            console.log('No courses found in path');
          }
        },
        error: (err) => console.error('Error fetching paths:', err)
      });
    } else {
      console.log('No learning path context - standalone course');
      alert('Course completed!');
      this.router.navigate(['/employee/courses']);
    }
  }
}
