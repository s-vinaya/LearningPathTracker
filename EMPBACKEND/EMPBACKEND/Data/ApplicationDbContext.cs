using Microsoft.EntityFrameworkCore;
using EMPBACKEND.Models;

namespace EMPBACKEND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<UserAssessment> UserAssessments { get; set; }
        public DbSet<DailyGoal> DailyGoals { get; set; }
        public DbSet<LearningPlan> LearningPlans { get; set; }
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<LearningPathCourse> LearningPathCourses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseProgress> CourseProgresses { get; set; }
        public DbSet<AssessmentAttempt> AssessmentAttempts { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<VideoRequest> VideoRequests { get; set; }
        public DbSet<VideoProgress> VideoProgresses { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Enrollment configurations
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();

            // VideoProgress configurations
            modelBuilder.Entity<VideoProgress>()
                .HasIndex(vp => new { vp.UserId, vp.VideoId })
                .IsUnique();

            // DailyGoal configurations
            modelBuilder.Entity<DailyGoal>()
                .HasIndex(dg => new { dg.UserId, dg.Date, dg.GoalType })
                .IsUnique();

            // CourseProgress foreign key configurations to prevent cascade path conflicts
            modelBuilder.Entity<CourseProgress>()
                .HasOne(cp => cp.Course)
                .WithMany()
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal precision
            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Progress)
                .HasPrecision(5, 2);

            modelBuilder.Entity<CourseProgress>()
                .Property(cp => cp.PercentComplete)
                .HasPrecision(5, 2);
        }
    }
}