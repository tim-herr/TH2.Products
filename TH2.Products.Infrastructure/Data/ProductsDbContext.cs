using Microsoft.EntityFrameworkCore;
using TH2.Products.Domain.Entities;

namespace TH2.Products.Infrastructure.Data;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            
            // Configure foreign key relationship
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for foreign key joins and category filtering
            entity.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Product_CategoryId");

            // Index for active product filtering (soft-delete pattern)
            entity.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Product_IsActive");

            // Index for price range filtering and price-based sorting
            entity.HasIndex(p => p.Price)
                .HasDatabaseName("IX_Product_Price");

            // Index for temporal sorting operations
            entity.HasIndex(p => p.CreatedDate)
                .HasDatabaseName("IX_Product_CreatedDate");

            // Index for in-stock filtering and quantity-based sorting
            entity.HasIndex(p => p.StockQuantity)
                .HasDatabaseName("IX_Product_StockQuantity");

            // Index for text search performance on product names
            entity.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Product_Name");

            // Composite index covering most common search pattern: active products by category with price range
            entity.HasIndex(p => new { p.IsActive, p.CategoryId, p.Price })
                .HasDatabaseName("IX_Product_Active_Category_Price");
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(1000);
            
            // Index for active category filtering
            entity.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Category_IsActive");
        });
    }
}
