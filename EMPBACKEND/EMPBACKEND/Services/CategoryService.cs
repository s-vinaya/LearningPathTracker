using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Models;

namespace EMPBACKEND.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedDate = c.CreatedDate
            });
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category != null ? new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedDate = category.CreatedDate
            } : null;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                CreatedDate = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(category);
            return new CategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreatedDate = created.CreatedDate
            };
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = new Category
            {
                Id = id,
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };
            var updated = await _repository.UpdateAsync(category);
            return new CategoryDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                CreatedDate = updated.CreatedDate
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}