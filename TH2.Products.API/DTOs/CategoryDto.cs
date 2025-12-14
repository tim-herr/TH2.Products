using System.ComponentModel.DataAnnotations;

namespace TH2.Products.API.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category description is required")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Category description must be between 1 and 1000 characters")]
    public string Description { get; set; } = string.Empty;
}
