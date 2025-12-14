using TH2.Products.Domain.Entities;
using TH2.Products.Domain.Interfaces;

namespace TH2.Products.Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
    {
        return await _productRepository.GetAllActiveAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        return await _productRepository.CreateAsync(product);
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortOrder,
        int pageNumber,
        int pageSize)
    {
        return await _productRepository.SearchAsync(
            searchTerm,
            categoryId,
            minPrice,
            maxPrice,
            inStock,
            sortBy,
            sortOrder,
            pageNumber,
            pageSize);
    }
}
