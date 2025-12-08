using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Application.Services;
using learning_path_tracker.Data;
using learning_path_tracker.Data.Repositories;
using learning_path_tracker.Database;
using learning_path_tracker.Application.Logging;
using learning_path_tracker.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
        b => b.MigrationsAssembly("learning_path_tracker.Api")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ILearningPathRepository, LearningPathRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILearningPathService, LearningPathService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IManagerDashboardService, ManagerDashboardService>();
builder.Services.AddScoped<IManagerAssignmentService, ManagerAssignmentService>();
builder.Services.AddScoped<IManagerApprovalService, ManagerApprovalService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IPlatformSettingsService, PlatformSettingsService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<PointsCalculationService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<learning_path_tracker.Api.Services.CertificateGeneratorService>();

builder.Services.AddSingleton(new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "Logs", "app.log")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Departments.Any())
    {   
        var departments = new[]
        {
            new learning_path_tracker.Domain.Entities.Department { Name = "Engineering", Description = "Software development and engineering", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new learning_path_tracker.Domain.Entities.Department { Name = "Marketing", Description = "Marketing and brand management", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new learning_path_tracker.Domain.Entities.Department { Name = "Sales", Description = "Sales and business development", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new learning_path_tracker.Domain.Entities.Department { Name = "HR", Description = "Human resources and recruitment", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new learning_path_tracker.Domain.Entities.Department { Name = "Operations", Description = "Operations and logistics", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true },
            new learning_path_tracker.Domain.Entities.Department { Name = "Finance", Description = "Finance and accounting", CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30), IsActive = true }
        };
        context.Departments.AddRange(departments);
        context.SaveChanges();
    }
    var enrollmentsToFix = context.Enrollments.Where(e => e.Progress >= 80 && e.CompletedAt == null).ToList();
    if (enrollmentsToFix.Any())
    {
        foreach (var enrollment in enrollmentsToFix)
        {
            enrollment.CompletedAt = DateTime.UtcNow;
        }
        context.SaveChanges();
    }
    var pointsService = scope.ServiceProvider.GetRequiredService<PointsCalculationService>();
    pointsService.RecalculatePointsForExistingCompletions().Wait();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
