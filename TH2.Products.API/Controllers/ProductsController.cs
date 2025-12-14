using Microsoft.AspNetCore.Mvc;
using TH2.Products.API.DTOs;
using TH2.Products.Domain.Services;
using TH2.Products.Domain.Entities;

namespace TH2.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all active products with category info
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllActiveAsync();
        return Ok(products.Select(MapToDto));
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        
        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found or is inactive" });

        return Ok(MapToDto(product));
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            CategoryId = createDto.CategoryId,
            StockQuantity = createDto.StockQuantity
        };

        var createdProduct = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, MapToDto(createdProduct));
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Id = id,
            Name = updateDto.Name,
            Description = updateDto.Description,
            Price = updateDto.Price,
            CategoryId = updateDto.CategoryId,
            StockQuantity = updateDto.StockQuantity
        };

        var updatedProduct = await _productService.UpdateAsync(product);
        
        if (updatedProduct == null)
            return NotFound(new { message = $"Product with ID {id} not found or is inactive" });

        return Ok(MapToDto(updatedProduct));
    }

    /// <summary>
    /// Soft delete a product (sets IsActive to false)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Product with ID {id} not found or is inactive" });

        return NoContent();
    }

    /// <summary>
    /// Search for products with optional filters, sorting, and pagination
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> SearchProducts([FromQuery] ProductSearchDto searchDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (products, totalCount) = await _productService.SearchAsync(
            searchDto.SearchTerm,
            searchDto.CategoryId,
            searchDto.MinPrice,
            searchDto.MaxPrice,
            searchDto.InStock,
            searchDto.SortBy,
            searchDto.SortOrder,
            searchDto.PageNumber,
            searchDto.PageSize);

        var productDtos = products.Select(MapToDto);

        var result = new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize)
        };

        return Ok(result);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate
        };
    }
}
