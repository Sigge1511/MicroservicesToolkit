using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using ProductService.Models;

namespace ProductService.Tests;

public class ProductServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/products");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetAllProducts_ReturnsListOfProducts()
    {
        var response = await _client.GetAsync("/api/products");
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<Product>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(products);
        Assert.NotEmpty(products);
        Assert.All(products, p =>
        {
            Assert.True(p.Id > 0);
            Assert.False(string.IsNullOrEmpty(p.Name));
            Assert.True(p.Price > 0);
        });
    }

    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenProductExists()
    {
        var response = await _client.GetAsync("/api/products/1");
        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.EnsureSuccessStatusCode();
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/products/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetProductById_ReturnsCorrectProduct_ForMultipleIds(int productId)
    {
        var response = await _client.GetAsync($"/api/products/{productId}");
        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        response.EnsureSuccessStatusCode();
        Assert.NotNull(product);
        Assert.Equal(productId, product.Id);
    }
}
