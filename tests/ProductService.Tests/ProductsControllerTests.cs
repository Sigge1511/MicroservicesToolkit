using Xunit;
using Moq;
using ProductService.Controllers;
using ProductService.Models;
using ProductService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Tests;

public class ProductsControllerTests
{
    private readonly Mock<LoggingClient> _mockLoggingClient;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockLoggingClient = new Mock<LoggingClient>(null!, null!);
        _controller = new ProductsController(_mockLoggingClient.Object);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
    {
        var result = await _controller.GetAllProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetProductById_ReturnsOkResult_WhenProductExists()
    {
        var result = await _controller.GetProductById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, product.Id);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var result = await _controller.GetProductById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllProducts_LogsInfoMessage()
    {
        await _controller.GetAllProducts();

        _mockLoggingClient.Verify(
            x => x.LogInfoAsync(It.Is<string>(s => s.Contains("Get all products"))),
            Times.Once);
    }

    [Fact]
    public async Task GetProductById_LogsErrorMessage_WhenProductNotFound()
    {
        await _controller.GetProductById(999);

        _mockLoggingClient.Verify(
            x => x.LogErrorAsync(It.Is<string>(s => s.Contains("Product not found"))),
            Times.Once);
    }
}
