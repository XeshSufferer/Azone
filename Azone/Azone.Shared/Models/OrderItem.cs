namespace Azone.Shared.Models;

public class OrderItem
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public int ItemId { get; set; }
    public Item Item { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}