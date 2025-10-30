using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using EMPBACKEND.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class DepartmentRepository : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            return departments.Select(MapToDto);
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            return department != null ? MapToDto(department) : null;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDto)
        {
            var department = new Department
            {
                Name = createDto.Name,
                Description = createDto.Description
            };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return MapToDto(department);
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto updateDto)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) throw new ArgumentException("Department not found");
            
            existing.Name = updateDto.Name;
            existing.Description = updateDto.Description;
            
            await _context.SaveChangesAsync();
            return MapToDto(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return false;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }

        private static DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                CreatedDate = department.CreatedDate
            };
        }
    }
}