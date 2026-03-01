using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly LoggingClient _loggingClient;
    private readonly ProductServiceClient _productServiceClient;
    
    // In-memory order storage
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    public OrdersController(
        LoggingClient loggingClient,
        ProductServiceClient productServiceClient)
    {
        _loggingClient = loggingClient;
        _productServiceClient = productServiceClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        await _loggingClient.LogInfoAsync("Get all orders called");
        return Ok(_orders);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        await _loggingClient.LogInfoAsync($"Create order called for ProductId: {request.ProductId}");

        // Validate product exists
        var productExists = await _productServiceClient.ProductExistsAsync(request.ProductId);
        
        if (!productExists)
        {
            await _loggingClient.LogErrorAsync($"Product not found: {request.ProductId}");
            return BadRequest(new { message = "Product not found" });
        }

        await _loggingClient.LogInfoAsync($"Product found: {request.ProductId}");

        var order = new Order
        {
            Id = _nextId++,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);
        
        await _loggingClient.LogInfoAsync($"Order created successfully: OrderId {order.Id}");

        return CreatedAtAction(nameof(GetAllOrders), new { id = order.Id }, order);
    }
}

public class CreateOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
