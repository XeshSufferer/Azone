namespace Azone.Shared.Models;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    public User User { get; set; }
    public Item Item { get; set; }
}