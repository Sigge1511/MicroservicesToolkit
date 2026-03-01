using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly LoggingClient _loggingClient;
    
    // In-memory product storage
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 12999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 299.99m },
        new Product { Id = 3, Name = "Keyboard", Price = 799.99m },
        new Product { Id = 4, Name = "Monitor", Price = 3499.99m },
        new Product { Id = 5, Name = "Headphones", Price = 1299.99m }
    };

    public ProductsController(LoggingClient loggingClient)
    {
        _loggingClient = loggingClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        await _loggingClient.LogInfoAsync("Get all products called");
        return Ok(_products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        await _loggingClient.LogInfoAsync($"Get product by id called: {id}");
        
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            await _loggingClient.LogErrorAsync($"Product not found: {id}");
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }
}
