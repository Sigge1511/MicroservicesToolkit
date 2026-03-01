using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalLogs { get; set; }
    public List<LogEntry> RecentLogs { get; set; } = new();

    public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task OnGetAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var productServiceUrl = _configuration["Services:ProductService"] ?? "http://localhost:5001";
        var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5002";
        var loggingServiceUrl = _configuration["Services:LoggingService"] ?? "http://localhost:5003";

        try
        {
            var productsResponse = await httpClient.GetAsync($"{productServiceUrl}/api/products");
            if (productsResponse.IsSuccessStatusCode)
            {
                var json = await productsResponse.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TotalProducts = products?.Count ?? 0;
            }
        }
        catch { }

        try
        {
            var ordersResponse = await httpClient.GetAsync($"{orderServiceUrl}/api/orders");
            if (ordersResponse.IsSuccessStatusCode)
            {
                var json = await ordersResponse.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TotalOrders = orders?.Count ?? 0;
            }
        }
        catch { }

        try
        {
            var logsResponse = await httpClient.GetAsync($"{loggingServiceUrl}/api/logs?limit=10");
            if (logsResponse.IsSuccessStatusCode)
            {
                var json = await logsResponse.Content.ReadAsStringAsync();
                var logs = JsonSerializer.Deserialize<List<LogEntry>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                RecentLogs = logs ?? new List<LogEntry>();
                TotalLogs = RecentLogs.Count;
            }
        }
        catch { }
    }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class LogEntry
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
