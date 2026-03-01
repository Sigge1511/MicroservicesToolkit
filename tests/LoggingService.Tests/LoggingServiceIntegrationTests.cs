using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using LoggingService.Models;

namespace LoggingService.Tests;

public class LoggingServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LoggingServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLog_ReturnsSuccess()
    {
        var request = new
        {
            ServiceName = "TestService",
            Level = "INFO",
            Message = "Integration test log",
            Time = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/logs", content);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetLogs_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/logs");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Theory]
    [InlineData("INFO")]
    [InlineData("ERROR")]
    [InlineData("WARNING")]
    public async Task CreateLog_AcceptsDifferentLevels(string level)
    {
        var request = new
        {
            ServiceName = "TestService",
            Level = level,
            Message = $"Test {level} message",
            Time = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/logs", content);

        response.EnsureSuccessStatusCode();
    }
}
