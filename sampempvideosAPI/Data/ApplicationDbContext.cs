using Microsoft.EntityFrameworkCore;
using EMPBACKEND.Models;

namespace EMPBACKEND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Assessment> Assessments { get; set; } = null!;
        public DbSet<UserAssessment> UserAssessments { get; set; } = null!;
        public DbSet<DailyGoal> DailyGoals { get; set; } = null!;
        public DbSet<LearningPlan> LearningPlans { get; set; } = null!;
        public DbSet<VideoRequest> VideoRequests { get; set; } = null!;
        public DbSet<VideoProgress> VideoProgresses { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Salt).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("Employee");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.IsApproved).HasDefaultValue(false);
                entity.Property(u => u.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.UpdatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasOne(u => u.Department).WithMany(d => d.Users).HasForeignKey(u => u.DepartmentId).OnDelete(DeleteBehavior.SetNull);
            });

            // Department configurations
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Description).HasMaxLength(500);
                entity.Property(d => d.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(d => d.Name).IsUnique();
            });

            // Category configurations
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // Course configurations
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Description).HasMaxLength(1000);
                entity.Property(c => c.Duration).IsRequired();
                entity.Property(c => c.CreatedBy).IsRequired();
                entity.Property(c => c.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.IsActive).HasDefaultValue(true);
                entity.HasOne(c => c.Category).WithMany(cat => cat.Courses).HasForeignKey(c => c.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => c.CategoryId);
                entity.HasIndex(c => c.Title);
            });

            // Video configurations
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Title).IsRequired().HasMaxLength(200);
                entity.Property(v => v.VideoUrl).IsRequired().HasMaxLength(500);
                entity.Property(v => v.Duration).IsRequired();
                entity.Property(v => v.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(v => v.Course).WithMany(c => c.Videos).HasForeignKey(v => v.CourseId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(v => v.CourseId);
                entity.HasIndex(v => v.Title);
            });

            // VideoProgress configurations
            modelBuilder.Entity<VideoProgress>(entity =>
            {
                entity.HasKey(vp => vp.Id);
                entity.Property(vp => vp.WatchedDuration).IsRequired().HasDefaultValue(0);
                entity.Property(vp => vp.IsCompleted).HasDefaultValue(false);
                entity.Property(vp => vp.LastWatchedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(vp => vp.User).WithMany(u => u.VideoProgresses).HasForeignKey(vp => vp.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(vp => vp.Video).WithMany(v => v.VideoProgresses).HasForeignKey(vp => vp.VideoId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(vp => new { vp.UserId, vp.VideoId }).IsUnique();
                entity.HasIndex(vp => vp.UserId);
                entity.HasIndex(vp => vp.VideoId);
                entity.HasIndex(vp => vp.IsCompleted);
            });

            // VideoRequest configurations
            modelBuilder.Entity<VideoRequest>(entity =>
            {
                entity.HasKey(vr => vr.Id);
                entity.Property(vr => vr.VideoTitle).IsRequired().HasMaxLength(200);
                entity.Property(vr => vr.RequestDescription).HasMaxLength(1000);
                entity.Property(vr => vr.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(vr => vr.RequestedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(vr => vr.User).WithMany(u => u.VideoRequests).HasForeignKey(vr => vr.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(vr => vr.UserId);
                entity.HasIndex(vr => vr.Status);
                entity.HasIndex(vr => vr.RequestedDate);
            });

            // Enrollment configurations
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EnrolledDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Progress).HasPrecision(5, 2).HasDefaultValue(0);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("InProgress");
                entity.HasOne(e => e.User).WithMany(u => u.Enrollments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
                entity.HasIndex(e => e.Status);
            });

            // Assessment configurations
            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Questions).IsRequired();
                entity.Property(a => a.PassingScore).HasDefaultValue(70);
                entity.Property(a => a.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(a => a.Course).WithMany(c => c.Assessments).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(a => a.CourseId);
            });

            // UserAssessment configurations
            modelBuilder.Entity<UserAssessment>(entity =>
            {
                entity.HasKey(ua => ua.Id);
                entity.Property(ua => ua.Score).IsRequired();
                entity.Property(ua => ua.AttemptDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(ua => ua.IsPassed).IsRequired();
                entity.HasOne(ua => ua.User).WithMany(u => u.UserAssessments).HasForeignKey(ua => ua.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ua => ua.Assessment).WithMany(a => a.UserAssessments).HasForeignKey(ua => ua.AssessmentId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(ua => ua.UserId);
                entity.HasIndex(ua => ua.AssessmentId);
            });

            // DailyGoal configurations
            modelBuilder.Entity<DailyGoal>(entity =>
            {
                entity.HasKey(dg => dg.Id);
                entity.Property(dg => dg.GoalType).IsRequired().HasMaxLength(100);
                entity.Property(dg => dg.TargetValue).IsRequired();
                entity.Property(dg => dg.CurrentValue).HasDefaultValue(0);
                entity.Property(dg => dg.Date).HasDefaultValueSql("CAST(GETDATE() AS DATE)");
                entity.Property(dg => dg.IsCompleted).HasDefaultValue(false);
                entity.HasOne(dg => dg.User).WithMany(u => u.DailyGoals).HasForeignKey(dg => dg.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(dg => new { dg.UserId, dg.Date, dg.GoalType }).IsUnique();
                entity.HasIndex(dg => dg.Date);
            });

            // LearningPlan configurations
            modelBuilder.Entity<LearningPlan>(entity =>
            {
                entity.HasKey(lp => lp.Id);
                entity.Property(lp => lp.AssignedBy).IsRequired();
                entity.Property(lp => lp.AssignedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(lp => lp.DueDate).IsRequired();
                entity.Property(lp => lp.Status).HasMaxLength(50).HasDefaultValue("Assigned");
                entity.HasOne(lp => lp.User).WithMany(u => u.LearningPlans).HasForeignKey(lp => lp.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(lp => lp.Course).WithMany(c => c.LearningPlans).HasForeignKey(lp => lp.CourseId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(lp => lp.UserId);
                entity.HasIndex(lp => lp.CourseId);
                entity.HasIndex(lp => lp.Status);
            });

            // OtpCode configurations
            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Email).IsRequired().HasMaxLength(255);
                entity.Property(o => o.Code).IsRequired().HasMaxLength(6);
                entity.Property(o => o.ExpiryTime).IsRequired();
                entity.Property(o => o.IsUsed).HasDefaultValue(false);
                entity.HasIndex(o => o.Email);
                entity.HasIndex(o => o.Code);
            });

            // Check constraints for data integrity
            modelBuilder.Entity<Video>().ToTable(t => t.HasCheckConstraint("CK_Video_Duration", "Duration > 0 AND Duration <= 86400"));
            modelBuilder.Entity<VideoProgress>().ToTable(t => t.HasCheckConstraint("CK_VideoProgress_WatchedDuration", "WatchedDuration >= 0 AND WatchedDuration <= 86400"));
            modelBuilder.Entity<VideoRequest>().ToTable(t => t.HasCheckConstraint("CK_VideoRequest_Status", "Status IN ('Pending', 'Approved', 'Rejected')"));
            modelBuilder.Entity<Course>().ToTable(t => t.HasCheckConstraint("CK_Course_Duration", "Duration > 0"));
            modelBuilder.Entity<Assessment>().ToTable(t => t.HasCheckConstraint("CK_Assessment_PassingScore", "PassingScore >= 0 AND PassingScore <= 100"));
            modelBuilder.Entity<UserAssessment>().ToTable(t => t.HasCheckConstraint("CK_UserAssessment_Score", "Score >= 0 AND Score <= 100"));
            modelBuilder.Entity<Enrollment>().ToTable(t => t.HasCheckConstraint("CK_Enrollment_Progress", "Progress >= 0 AND Progress <= 100"));
            modelBuilder.Entity<Enrollment>().ToTable(t => t.HasCheckConstraint("CK_Enrollment_Status", "Status IN ('InProgress', 'Completed', 'Dropped')"));
            modelBuilder.Entity<LearningPlan>().ToTable(t => t.HasCheckConstraint("CK_LearningPlan_Status", "Status IN ('Assigned', 'InProgress', 'Completed', 'Overdue')"));
            modelBuilder.Entity<DailyGoal>().ToTable(t => t.HasCheckConstraint("CK_DailyGoal_Values", "TargetValue > 0 AND CurrentValue >= 0"));
            modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint("CK_User_Role", "Role IN ('Admin', 'Manager', 'Employee')"));
        }
    }
}