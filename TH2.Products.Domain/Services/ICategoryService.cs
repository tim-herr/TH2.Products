using TH2.Products.Domain.Entities;

namespace TH2.Products.Domain.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<Category> CreateAsync(Category category);
}
