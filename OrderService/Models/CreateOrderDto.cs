namespace OrderService.Models
{
    public class CreateOrderDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
