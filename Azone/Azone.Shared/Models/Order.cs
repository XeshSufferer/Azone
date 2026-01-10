using Azone.Shared.Models.Enums;

namespace Azone.Shared.Models;

public class Order
{
    public int Id { get; set; }
    public ICollection<OrderItem> Items { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public int AuthorId { get; set; }
    public User Author { get; set; }
    
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "RUB"; // или как у вас
    
    public string ShippingAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}