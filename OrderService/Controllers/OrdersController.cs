using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;
using OrderService.Events;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly RabbitMqPublisher _publisher;

    public OrdersController(OrderDbContext context, RabbitMqPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var order = new Order
        {
            ProductName = dto.ProductName,
            Price = dto.Price
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var orderEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            Price = order.Price,
            ProductName = order.ProductName
        };

        _publisher.Publish(orderEvent);

        return Ok(order);
    }
}
