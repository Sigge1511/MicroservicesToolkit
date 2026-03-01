using System.Text.Json;

namespace OrderService.Services;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var productUrl = configuration["Services:ProductService"] 
            ?? "http://localhost:5001";
        _httpClient.BaseAddress = new Uri(productUrl);
    }

    public async Task<bool> ProductExistsAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{productId}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch
        {
            return null;
        }
    }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
