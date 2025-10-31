using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;
using AutoMapper;

namespace EMPBACKEND.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var created = await _repository.CreateAsync(category);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = id;
            var updated = await _repository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}