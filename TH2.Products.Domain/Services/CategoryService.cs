using TH2.Products.Domain.Entities;
using TH2.Products.Domain.Interfaces;

namespace TH2.Products.Domain.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllActiveAsync()
    {
        return await _categoryRepository.GetAllActiveAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        return await _categoryRepository.CreateAsync(category);
    }
}
