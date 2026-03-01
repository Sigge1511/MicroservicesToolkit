using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Web.Pages;

public class OrdersModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    public int Quantity { get; set; } = 1;

    public List<ProductDto>? Products { get; set; }
    public List<OrderDto>? Orders { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public OrdersModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
            return Page();
        }

        var httpClient = _httpClientFactory.CreateClient();
        var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5002";

        try
        {
            var orderRequest = new { ProductId, Quantity };
            var json = JsonSerializer.Serialize(orderRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{orderServiceUrl}/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                Message = "Order created successfully!";
                IsSuccess = true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Message = $"Failed to create order: {errorContent}";
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            Message = $"Error: {ex.Message}";
            IsSuccess = false;
        }

        await LoadDataAsync();
        return Page();
    }

    private async Task LoadDataAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var productServiceUrl = _configuration["Services:ProductService"] ?? "http://localhost:5001";
        var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5002";

        try
        {
            var productsResponse = await httpClient.GetAsync($"{productServiceUrl}/api/products");
            if (productsResponse.IsSuccessStatusCode)
            {
                var json = await productsResponse.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        catch { }

        try
        {
            var ordersResponse = await httpClient.GetAsync($"{orderServiceUrl}/api/orders");
            if (ordersResponse.IsSuccessStatusCode)
            {
                var json = await ordersResponse.Content.ReadAsStringAsync();
                Orders = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        catch { }
    }
}
