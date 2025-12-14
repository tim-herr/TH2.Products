namespace TH2.Products.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    #region GetProducts Tests

    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var products = GetTestProducts();
        _mockService.Setup(service => service.GetAllActiveAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenNoProducts()
    {
        // Arrange
        _mockService.Setup(service => service.GetAllActiveAsync())
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().BeEmpty();
    }

    #endregion

    #region GetProduct Tests

    [Fact]
    public async Task GetProduct_ReturnsOkResult_WithProduct_WhenProductExists()
    {
        // Arrange
        var product = GetTestProducts().First();
        _mockService.Setup(service => service.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Id.Should().Be(1);
        returnedProduct.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockService.Setup(service => service.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetProduct(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WithProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = 1,
            StockQuantity = 50
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            CategoryId = createDto.CategoryId,
            StockQuantity = createDto.StockQuantity,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        _mockService.Setup(service => service.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("New Product");
        returnedProduct.Price.Should().Be(99.99m);
    }

    #endregion

    #region UpdateProduct Tests

    [Fact]
    public async Task UpdateProduct_ReturnsOkResult_WithUpdatedProduct()
    {
        // Arrange
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            CategoryId = 1,
            StockQuantity = 75
        };

        var updatedProduct = new Product
        {
            Id = 1,
            Name = updateDto.Name,
            Description = updateDto.Description,
            Price = updateDto.Price,
            CategoryId = updateDto.CategoryId,
            Category = new Category { Id = 1, Name = "Electronics" },
            StockQuantity = updateDto.StockQuantity,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        _mockService.Setup(service => service.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("Updated Product");
        returnedProduct.Price.Should().Be(149.99m);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            CategoryId = 1,
            StockQuantity = 75
        };

        _mockService.Setup(service => service.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.UpdateProduct(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteProduct Tests

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductExists()
    {
        // Arrange
        _mockService.Setup(service => service.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockService.Setup(service => service.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region SearchProducts Tests

    [Fact]
    public async Task SearchProducts_ReturnsAllProducts_WhenNoFiltersApplied()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts();

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(3);
        returnedResult.TotalCount.Should().Be(3);
        returnedResult.PageNumber.Should().Be(1);
        returnedResult.PageSize.Should().Be(10);
        returnedResult.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task SearchProducts_FiltersBy_SearchTerm()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            SearchTerm = "Laptop",
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().Where(p => p.Name.Contains("Laptop"));

        _mockService.Setup(service => service.SearchAsync(
            "Laptop",
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            10))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(1);
        returnedResult.Items.First().Name.Should().Be("Laptop");
        returnedResult.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task SearchProducts_FiltersBy_CategoryId()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            CategoryId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().Where(p => p.CategoryId == 1);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            1,
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            10))
            .ReturnsAsync((products, 2));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(2);
        returnedResult.Items.Should().AllSatisfy(p => p.CategoryId.Should().Be(1));
    }

    [Fact]
    public async Task SearchProducts_FiltersBy_PriceRange()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            MinPrice = 50m,
            MaxPrice = 500m,
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().Where(p => p.Price >= 50m && p.Price <= 500m);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            50m,
            500m,
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            10))
            .ReturnsAsync((products, 0));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(0);
        returnedResult.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task SearchProducts_FiltersBy_InStock()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            InStock = true,
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().Where(p => p.StockQuantity > 0);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            true,
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            10))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(3);
        returnedResult.Items.Should().AllSatisfy(p => p.StockQuantity.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task SearchProducts_CombinesMultipleFilters()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            SearchTerm = "laptop",
            CategoryId = 1,
            MinPrice = 500m,
            MaxPrice = 1000m,
            InStock = true,
            SortBy = "price",
            SortOrder = "desc",
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().Where(p => p.Name.ToLower().Contains("laptop"));

        _mockService.Setup(service => service.SearchAsync(
            "laptop",
            1,
            500m,
            1000m,
            true,
            "price",
            "desc",
            1,
            10))
            .ReturnsAsync((products, 1));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(1);
        returnedResult.TotalCount.Should().Be(1);
        returnedResult.Items.First().Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task SearchProducts_SortsBy_Name_Ascending()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            SortBy = "name",
            SortOrder = "asc",
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().OrderBy(p => p.Name);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            "name",
            "asc",
            1,
            10))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        var productNames = returnedResult.Items.Select(p => p.Name).ToList();
        productNames.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task SearchProducts_SortsBy_Price_Descending()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            SortBy = "price",
            SortOrder = "desc",
            PageNumber = 1,
            PageSize = 10
        };

        var products = GetTestProducts().OrderByDescending(p => p.Price);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            "price",
            "desc",
            1,
            10))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        var productPrices = returnedResult.Items.Select(p => p.Price).ToList();
        productPrices.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task SearchProducts_HandlesPagination_FirstPage()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            PageNumber = 1,
            PageSize = 2
        };

        var products = GetTestProducts().Take(2);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            2))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(2);
        returnedResult.TotalCount.Should().Be(3);
        returnedResult.PageNumber.Should().Be(1);
        returnedResult.PageSize.Should().Be(2);
        returnedResult.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task SearchProducts_HandlesPagination_SecondPage()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            PageNumber = 2,
            PageSize = 2
        };

        var products = GetTestProducts().Skip(2).Take(2);

        _mockService.Setup(service => service.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            2,
            2))
            .ReturnsAsync((products, 3));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().HaveCount(1);
        returnedResult.TotalCount.Should().Be(3);
        returnedResult.PageNumber.Should().Be(2);
        returnedResult.PageSize.Should().Be(2);
        returnedResult.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task SearchProducts_ReturnsEmpty_WhenNoMatchingProducts()
    {
        // Arrange
        var searchDto = new ProductSearchDto
        {
            SearchTerm = "NonExistentProduct",
            PageNumber = 1,
            PageSize = 10
        };

        _mockService.Setup(service => service.SearchAsync(
            "NonExistentProduct",
            It.IsAny<int?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<bool?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            1,
            10))
            .ReturnsAsync((new List<Product>(), 0));

        // Act
        var result = await _controller.SearchProducts(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<ProductDto>>().Subject;
        returnedResult.Items.Should().BeEmpty();
        returnedResult.TotalCount.Should().Be(0);
        returnedResult.TotalPages.Should().Be(0);
    }

    #endregion

    #region Helper Methods

    private IEnumerable<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" },
                StockQuantity = 50,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                IsActive = true
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Description = "Latest model smartphone",
                Price = 699.99m,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" },
                StockQuantity = 100,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                IsActive = true
            },
            new Product
            {
                Id = 3,
                Name = "T-Shirt",
                Description = "Cotton t-shirt",
                Price = 19.99m,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Clothing" },
                StockQuantity = 200,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                IsActive = true
            }
        };
    }

    #endregion
}
