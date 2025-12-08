using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<LearningPath> LearningPaths { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<LearningPathCourse> LearningPathCourses { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<NotificationTracking> NotificationTrackings { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<CertificateRequest> CertificateRequests { get; set; }
    public DbSet<LearningSchedule> LearningSchedules { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<PlatformSettings> PlatformSettings { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<UserPoints> UserPoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LearningPathCourse>()
            .HasKey(lpc => new { lpc.LearningPathId, lpc.CourseId });

        modelBuilder.Entity<LearningPathCourse>()
            .HasOne(lpc => lpc.LearningPath)
            .WithMany(lp => lp.LearningPathCourses)
            .HasForeignKey(lpc => lpc.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LearningPathCourse>()
            .HasOne(lpc => lpc.Course)
            .WithMany(c => c.LearningPathCourses)
            .HasForeignKey(lpc => lpc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.User)
            .WithMany(u => u.Certificates)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Module>()
            .HasOne(m => m.LearningPath)
            .WithMany(lp => lp.Modules)
            .HasForeignKey(m => m.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationTracking>()
            .HasOne(nt => nt.Notification)
            .WithMany(n => n.Trackings)
            .HasForeignKey(nt => nt.NotificationId);

        modelBuilder.Entity<NotificationTracking>()
            .HasOne(nt => nt.User)
            .WithMany()
            .HasForeignKey(nt => nt.UserId);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.AssignedByManager)
            .WithMany()
            .HasForeignKey(a => a.AssignedByManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Approval>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Course)
            .WithMany()
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.LearningPath)
            .WithMany()
            .HasForeignKey(q => q.LearningPathId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CertificateRequest>()
            .HasOne(cr => cr.Employee)
            .WithMany()
            .HasForeignKey(cr => cr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CertificateRequest>()
            .HasOne(cr => cr.ReviewedByManager)
            .WithMany()
            .HasForeignKey(cr => cr.ReviewedByManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
