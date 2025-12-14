using Microsoft.EntityFrameworkCore;
using TH2.Products.Domain.Entities;
using TH2.Products.Domain.Interfaces;
using TH2.Products.Infrastructure.Data;

namespace TH2.Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        
        return product;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        product.IsActive = true;
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == product.Id && p.IsActive);
        
        if (existingProduct == null)
            return null;

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.StockQuantity = product.StockQuantity;

        await _context.SaveChangesAsync();
        
        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        
        if (product == null)
            return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();
        
        return true;
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
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchWords = searchTerm.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in searchWords)
            {
                var searchPattern = $"%{word}%";
                query = query.Where(p => 
                    EF.Functions.Like(p.Name, searchPattern) || 
                    EF.Functions.Like(p.Description, searchPattern));
            }
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStock.HasValue && inStock.Value)
        {
            query = query.Where(p => p.StockQuantity > 0);
        }

        var totalCount = await query.CountAsync();

        query = ApplySorting(query, sortBy, sortOrder);

        // DEBUGGING
        //string sql = query.ToQueryString();

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    private IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        var sortedQuery = (sortBy?.ToLower()) switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "createddate" => isDescending ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            "stockquantity" => isDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
            _ => query.OrderBy(p => p.Name)
        };

        return sortedQuery;
    }
}
