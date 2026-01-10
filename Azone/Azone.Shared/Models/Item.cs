namespace Azone.Shared.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Shop Shop { get; set; }
    public int ShopId { get; set; }
    
    public ICollection<ProductReview> ProductReviews { get; set; }
}