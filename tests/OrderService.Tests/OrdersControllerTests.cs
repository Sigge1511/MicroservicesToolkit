using Xunit;
using Moq;
using OrderService.Controllers;
using OrderService.Models;
using OrderService.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Tests;

public class OrdersControllerTests
{
    private readonly Mock<LoggingClient> _mockLoggingClient;
    private readonly Mock<ProductServiceClient> _mockProductClient;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockLoggingClient = new Mock<LoggingClient>(null!, null!);
        _mockProductClient = new Mock<ProductServiceClient>(null!, null!);
        _controller = new OrdersController(_mockLoggingClient.Object, _mockProductClient.Object);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsOkResult_WithEmptyListInitially()
    {
        var result = await _controller.GetAllOrders();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
        Assert.Empty(orders);
    }

    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenProductDoesNotExist()
    {
        _mockProductClient
            .Setup(x => x.ProductExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        var request = new CreateOrderRequest { ProductId = 999, Quantity = 1 };
        var result = await _controller.CreateOrder(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedResult_WhenProductExists()
    {
        _mockProductClient
            .Setup(x => x.ProductExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        var request = new CreateOrderRequest { ProductId = 1, Quantity = 2 };
        var result = await _controller.CreateOrder(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var order = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal(1, order.ProductId);
        Assert.Equal(2, order.Quantity);
    }

    [Fact]
    public async Task CreateOrder_LogsInfoMessages_OnSuccess()
    {
        _mockProductClient
            .Setup(x => x.ProductExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        var request = new CreateOrderRequest { ProductId = 1, Quantity = 1 };
        await _controller.CreateOrder(request);

        _mockLoggingClient.Verify(
            x => x.LogInfoAsync(It.Is<string>(s => s.Contains("Create order called"))),
            Times.Once);
        _mockLoggingClient.Verify(
            x => x.LogInfoAsync(It.Is<string>(s => s.Contains("Product found"))),
            Times.Once);
        _mockLoggingClient.Verify(
            x => x.LogInfoAsync(It.Is<string>(s => s.Contains("Order created successfully"))),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrder_LogsErrorMessage_WhenProductNotFound()
    {
        _mockProductClient
            .Setup(x => x.ProductExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        var request = new CreateOrderRequest { ProductId = 999, Quantity = 1 };
        await _controller.CreateOrder(request);

        _mockLoggingClient.Verify(
            x => x.LogErrorAsync(It.Is<string>(s => s.Contains("Product not found"))),
            Times.Once);
    }
}
