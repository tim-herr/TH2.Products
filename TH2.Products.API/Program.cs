using Microsoft.EntityFrameworkCore;
using TH2.Products.API.Middleware;
using TH2.Products.Domain.Services;
using TH2.Products.Domain.Interfaces;
using TH2.Products.Infrastructure.Data;
using TH2.Products.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite database
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=products.db"));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Register domain services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Seed data if database is empty
    if (!dbContext.Categories.Any())
    {
        var categories = new[]
        {
            new TH2.Products.Domain.Entities.Category { Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
            new TH2.Products.Domain.Entities.Category { Name = "Clothing", Description = "Apparel and fashion items", IsActive = true },
            new TH2.Products.Domain.Entities.Category { Name = "Books", Description = "Books and publications", IsActive = true },
            new TH2.Products.Domain.Entities.Category { Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
            new TH2.Products.Domain.Entities.Category { Name = "Sports & Fitness", Description = "Sports equipment and fitness gear", IsActive = true }
        };
        
        dbContext.Categories.AddRange(categories);
        dbContext.SaveChanges();
        
        var products = new[]
        {
            // Electronics
            new TH2.Products.Domain.Entities.Product { Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, CategoryId = 1, StockQuantity = 50, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Smartphone", Description = "Latest model smartphone", Price = 699.99m, CategoryId = 1, StockQuantity = 100, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Wireless Headphones", Description = "Noise-cancelling Bluetooth headphones", Price = 149.99m, CategoryId = 1, StockQuantity = 75, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Tablet", Description = "10-inch tablet with stylus", Price = 399.99m, CategoryId = 1, StockQuantity = 60, IsActive = true },
            
            // Clothing
            new TH2.Products.Domain.Entities.Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, CategoryId = 2, StockQuantity = 200, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Jeans", Description = "Denim jeans", Price = 49.99m, CategoryId = 2, StockQuantity = 150, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Winter Jacket", Description = "Waterproof winter jacket", Price = 129.99m, CategoryId = 2, StockQuantity = 80, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Winter Boots", Description = "Warm winter boots", Price = 79.99m, CategoryId = 2, StockQuantity = 120, IsActive = true },
            
            // Books
            new TH2.Products.Domain.Entities.Product { Name = "Novel", Description = "Bestselling fiction novel", Price = 14.99m, CategoryId = 3, StockQuantity = 75, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Cookbook", Description = "Gourmet cooking recipes", Price = 24.99m, CategoryId = 3, StockQuantity = 60, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Self-Help Book", Description = "Personal development guide", Price = 18.99m, CategoryId = 3, StockQuantity = 90, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Programming Guide", Description = "Complete guide to modern programming", Price = 49.99m, CategoryId = 3, StockQuantity = 45, IsActive = true },
            
            // Home & Garden
            new TH2.Products.Domain.Entities.Product { Name = "Garden Hose", Description = "50ft expandable garden hose", Price = 29.99m, CategoryId = 4, StockQuantity = 40, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Plant Pot", Description = "Ceramic plant pot", Price = 12.99m, CategoryId = 4, StockQuantity = 80, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Lawn Mower", Description = "Electric lawn mower", Price = 249.99m, CategoryId = 4, StockQuantity = 25, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Tool Set", Description = "50-piece home tool set", Price = 89.99m, CategoryId = 4, StockQuantity = 55, IsActive = true },
            
            // Sports & Fitness
            new TH2.Products.Domain.Entities.Product { Name = "Yoga Mat", Description = "Non-slip exercise yoga mat", Price = 34.99m, CategoryId = 5, StockQuantity = 100, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Dumbbells Set", Description = "Adjustable dumbbells 5-50 lbs", Price = 199.99m, CategoryId = 5, StockQuantity = 40, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Resistance Bands", Description = "Set of 5 resistance bands", Price = 24.99m, CategoryId = 5, StockQuantity = 85, IsActive = true },
            new TH2.Products.Domain.Entities.Product { Name = "Jump Rope", Description = "Speed jump rope with counter", Price = 15.99m, CategoryId = 5, StockQuantity = 110, IsActive = true }
        };
        
        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Commented out to allow HTTP requests during testing
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();
