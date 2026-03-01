using System.Text;
using System.Text.Json;

namespace ProductService.Services;

public class LoggingClient
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceName;

    public LoggingClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _serviceName = "ProductService";
        
        var loggingUrl = configuration["Services:LoggingService"] 
            ?? "http://localhost:5003";
        _httpClient.BaseAddress = new Uri(loggingUrl);
    }

    public async Task LogAsync(string level, string message)
    {
        try
        {
            var logEntry = new
            {
                ServiceName = _serviceName,
                Level = level,
                Message = message,
                Time = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(logEntry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            await _httpClient.PostAsync("/api/logs", content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send log: {ex.Message}");
        }
    }

    public Task LogInfoAsync(string message) => LogAsync("INFO", message);
    public Task LogErrorAsync(string message) => LogAsync("ERROR", message);
}
