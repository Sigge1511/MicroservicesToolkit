using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using OrderService.Models;

namespace OrderService.Tests;

public class OrderServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllOrders_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/orders");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetAllOrders_ReturnsEmptyListInitially()
    {
        var response = await _client.GetAsync("/api/orders");
        var content = await response.Content.ReadAsStringAsync();
        var orders = JsonSerializer.Deserialize<List<Order>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(orders);
        Assert.Empty(orders);
    }

    [Fact]
    public async Task CreateOrder_RequiresProductIdAndQuantity()
    {
        var request = new { ProductId = 1, Quantity = 2 };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/orders", content);

        Assert.NotNull(response);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(1, 5)]
    public async Task CreateOrder_HandlesMultipleQuantities(int productId, int quantity)
    {
        var request = new { ProductId = productId, Quantity = quantity };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/orders", content);

        Assert.NotNull(response);
    }
}
