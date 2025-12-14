using TH2.Products.Domain.Entities;

namespace TH2.Products.Domain.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllActiveAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortOrder,
        int pageNumber,
        int pageSize);
}
