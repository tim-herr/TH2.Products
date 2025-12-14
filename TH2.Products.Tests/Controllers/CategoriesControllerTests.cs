namespace TH2.Products.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoriesController(_mockService.Object);
    }

    #region GetCategories Tests

    [Fact]
    public async Task GetCategories_ReturnsOkResult_WithListOfCategories()
    {
        // Arrange
        var categories = GetTestCategories();
        _mockService.Setup(service => service.GetAllActiveAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDto>>().Subject;
        returnedCategories.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetCategories_ReturnsEmptyList_WhenNoCategories()
    {
        // Arrange
        _mockService.Setup(service => service.GetAllActiveAsync())
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDto>>().Subject;
        returnedCategories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategories_ReturnsCorrectCategoryData()
    {
        // Arrange
        var categories = GetTestCategories();
        _mockService.Setup(service => service.GetAllActiveAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDto>>().Subject.ToList();
        
        returnedCategories[0].Id.Should().Be(1);
        returnedCategories[0].Name.Should().Be("Electronics");
        returnedCategories[0].Description.Should().Be("Electronic devices and accessories");
        
        returnedCategories[1].Id.Should().Be(2);
        returnedCategories[1].Name.Should().Be("Clothing");
        returnedCategories[1].Description.Should().Be("Apparel and fashion items");
    }

    #endregion

    #region CreateCategory Tests

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtAction_WithCategory()
    {
        // Arrange
        var createDto = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "New category description"
        };

        var createdCategory = new Category
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            IsActive = true
        };

        _mockService.Setup(service => service.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _controller.CreateCategory(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedCategory = createdResult.Value.Should().BeOfType<CategoryDto>().Subject;
        returnedCategory.Name.Should().Be("New Category");
        returnedCategory.Description.Should().Be("New category description");
    }

    [Fact]
    public async Task CreateCategory_CallsServiceWithCorrectData()
    {
        // Arrange
        var createDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test description"
        };

        var createdCategory = new Category
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            IsActive = true
        };

        _mockService.Setup(service => service.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        await _controller.CreateCategory(createDto);

        // Assert
        _mockService.Verify(service => service.CreateAsync(It.Is<Category>(c =>
            c.Name == "Test Category" &&
            c.Description == "Test description"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtActionWithCorrectRoute()
    {
        // Arrange
        var createDto = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "New category description"
        };

        var createdCategory = new Category
        {
            Id = 5,
            Name = createDto.Name,
            Description = createDto.Description,
            IsActive = true
        };

        _mockService.Setup(service => service.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _controller.CreateCategory(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetCategories));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(5);
    }

    #endregion

    #region Helper Methods

    private IEnumerable<Category> GetTestCategories()
    {
        return new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices and accessories",
                IsActive = true
            },
            new Category
            {
                Id = 2,
                Name = "Clothing",
                Description = "Apparel and fashion items",
                IsActive = true
            },
            new Category
            {
                Id = 3,
                Name = "Books",
                Description = "Books and publications",
                IsActive = true
            }
        };
    }

    #endregion
}
