namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending"; // Pending / Paid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
