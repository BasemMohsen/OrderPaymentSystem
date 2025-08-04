namespace OrderService.Events;

public class PaymentCompletedEvent
{
    public int OrderId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
}
