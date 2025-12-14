using Microsoft.AspNetCore.Mvc;
using TH2.Products.API.DTOs;
using TH2.Products.Domain.Services;
using TH2.Products.Domain.Entities;

namespace TH2.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all active categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _categoryService.GetAllActiveAsync();
        return Ok(categories.Select(MapToDto));
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = new Category
        {
            Name = createDto.Name,
            Description = createDto.Description
        };

        var createdCategory = await _categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(GetCategories), new { id = createdCategory.Id }, MapToDto(createdCategory));
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
