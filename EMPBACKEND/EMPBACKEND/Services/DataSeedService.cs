using EMPBACKEND.Data;
using EMPBACKEND.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Services
{
    public class DataSeedService
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            await SeedAsync(context);
        }

        private static async Task SeedAsync(ApplicationDbContext _context)
        {
            // Seed Departments
            if (!_context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = "Information Technology", Description = "IT Department" },
                    new Department { Name = "Human Resources", Description = "HR Department" },
                    new Department { Name = "Finance", Description = "Finance Department" },
                    new Department { Name = "Marketing", Description = "Marketing Department" }
                };

                _context.Departments.AddRange(departments);
                await _context.SaveChangesAsync();
            }

            // Seed Categories
            if (!_context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Programming", Description = "Programming and Development" },
                    new Category { Name = "Management", Description = "Management and Leadership" },
                    new Category { Name = "Design", Description = "Design and Creative" },
                    new Category { Name = "Business", Description = "Business and Finance" }
                };

                _context.Categories.AddRange(categories);
                await _context.SaveChangesAsync();
            }

            // Seed Users
            if (!_context.Users.Any())
            {
                var itDept = _context.Departments.First(d => d.Name == "Information Technology");
                var hrDept = _context.Departments.First(d => d.Name == "Human Resources");

                var users = new[]
                {
                    new User
                    {
                        Username = "admin",
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@company.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Role = "Admin",
                        DepartmentId = itDept.Id,
                        IsActive = true,
                        IsApproved = true
                    },
                    new User
                    {
                        Username = "manager1",
                        FirstName = "Manager",
                        LastName = "One",
                        Email = "manager@company.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("manager123"),
                        Role = "Manager",
                        DepartmentId = itDept.Id,
                        IsActive = true,
                        IsApproved = true
                    },
                    new User
                    {
                        Username = "employee1",
                        FirstName = "Employee",
                        LastName = "One",
                        Email = "employee1@company.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("employee123"),
                        Role = "Employee",
                        DepartmentId = itDept.Id,
                        IsActive = true,
                        IsApproved = true
                    },
                    new User
                    {
                        Username = "employee2",
                        FirstName = "Employee",
                        LastName = "Two",
                        Email = "employee2@company.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("employee123"),
                        Role = "Employee",
                        DepartmentId = hrDept.Id,
                        IsActive = true,
                        IsApproved = true
                    }
                };

                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
            }

            // Seed Courses
            if (!_context.Courses.Any())
            {
                var programmingCategory = _context.Categories.First(c => c.Name == "Programming");
                var managementCategory = _context.Categories.First(c => c.Name == "Management");
                var admin = _context.Users.First(u => u.Role == "Admin");

                var courses = new[]
                {
                    new Course
                    {
                        Title = "Introduction to C# Programming",
                        Description = "Learn the basics of C# programming language",
                        CategoryId = programmingCategory.Id,
                        Duration = 120,
                        CreatedBy = admin.Id,
                        IsActive = true
                    },
                    new Course
                    {
                        Title = "Web Development with ASP.NET Core",
                        Description = "Build modern web applications with ASP.NET Core",
                        CategoryId = programmingCategory.Id,
                        Duration = 180,
                        CreatedBy = admin.Id,
                        IsActive = true
                    },
                    new Course
                    {
                        Title = "Leadership and Team Management",
                        Description = "Develop leadership skills and learn team management",
                        CategoryId = managementCategory.Id,
                        Duration = 90,
                        CreatedBy = admin.Id,
                        IsActive = true
                    }
                };

                _context.Courses.AddRange(courses);
                await _context.SaveChangesAsync();
            }
        }
    }
}