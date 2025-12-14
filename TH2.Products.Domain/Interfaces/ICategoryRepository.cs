using TH2.Products.Domain.Entities;

namespace TH2.Products.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<Category> CreateAsync(Category category);
}
