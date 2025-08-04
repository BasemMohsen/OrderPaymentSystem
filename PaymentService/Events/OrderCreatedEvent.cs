namespace PaymentService.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public decimal Price { get; set; }
    public string ProductName { get; set; } = string.Empty;
}
