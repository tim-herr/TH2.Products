using Microsoft.EntityFrameworkCore;
using TH2.Products.Domain.Entities;
using TH2.Products.Domain.Interfaces;
using TH2.Products.Infrastructure.Data;

namespace TH2.Products.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductsDbContext _context;

    public CategoryRepository(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllActiveAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        category.IsActive = true;
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        return category;
    }
}
