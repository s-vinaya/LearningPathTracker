using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using EMPBACKEND.Data;
using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream
=======
using AutoMapper;
>>>>>>> Stashed changes

namespace EMPBACKEND.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
<<<<<<< Updated upstream

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
=======
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _context.Departments.ToListAsync();
<<<<<<< Updated upstream
            return departments.Select(MapToDto);
=======
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
>>>>>>> Stashed changes
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
<<<<<<< Updated upstream
            return department != null ? MapToDto(department) : null;
=======
            return department != null ? _mapper.Map<DepartmentDto>(department) : null;
>>>>>>> Stashed changes
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDto)
        {
<<<<<<< Updated upstream
            var department = new Department
            {
                Name = createDto.Name,
                Description = createDto.Description
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return MapToDto(department);
=======
            var department = _mapper.Map<Department>(createDto);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
>>>>>>> Stashed changes
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto updateDto)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) throw new ArgumentException("Department not found");

<<<<<<< Updated upstream
            existing.Name = updateDto.Name;
            existing.Description = updateDto.Description;

            await _context.SaveChangesAsync();
            return MapToDto(existing);
=======
            _mapper.Map(updateDto, existing);

            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(existing);
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
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
=======

>>>>>>> Stashed changes
    }
}