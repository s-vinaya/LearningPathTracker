using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using EMPBACKEND.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            return department != null ? _mapper.Map<DepartmentDto>(department) : null;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDto)
        {
            var department = _mapper.Map<Department>(createDto);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto updateDto)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) throw new ArgumentException("Department not found");

            _mapper.Map(updateDto, existing);

            await _context.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(existing);
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


    }
}