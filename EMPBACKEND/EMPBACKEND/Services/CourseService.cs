using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courses = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            return course != null ? _mapper.Map<CourseDto>(course) : null;
        }

        public async Task<IEnumerable<CourseDto>> GetByCategoryAsync(int categoryId)
        {
            var courses = await _repository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto createDto)
        {
            var course = _mapper.Map<Course>(createDto);

            var created = await _repository.CreateAsync(course);
            var result = await _repository.GetByIdAsync(created.Id);
            return _mapper.Map<CourseDto>(result!);
        }

        public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("Course not found");

            _mapper.Map(updateDto, existing);

            await _repository.UpdateAsync(existing);
            var updated = await _repository.GetByIdAsync(id);
            return _mapper.Map<CourseDto>(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }


    }
}