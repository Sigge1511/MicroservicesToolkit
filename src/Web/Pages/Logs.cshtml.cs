using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages;

public class LogsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public List<LogEntry>? Logs { get; set; }
    public string? ServiceFilter { get; set; }
    public string? LevelFilter { get; set; }

    public LogsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task OnGetAsync(string? serviceName = null, string? level = null)
    {
        ServiceFilter = serviceName;
        LevelFilter = level;

        var httpClient = _httpClientFactory.CreateClient();
        var loggingServiceUrl = _configuration["Services:LoggingService"] ?? "http://localhost:5003";

        try
        {
            var queryParams = new List<string> { "limit=50" };

            if (!string.IsNullOrEmpty(serviceName))
            {
                queryParams.Add($"serviceName={serviceName}");
            }

            if (!string.IsNullOrEmpty(level))
            {
                queryParams.Add($"level={level}");
            }

            var query = string.Join("&", queryParams);
            var response = await httpClient.GetAsync($"{loggingServiceUrl}/api/logs?{query}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Logs = JsonSerializer.Deserialize<List<LogEntry>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching logs: {ex.Message}");
        }
    }
}
