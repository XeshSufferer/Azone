namespace Azone.Shared.Models;

public class ShopReview
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Shop Shop { get; set; }
    public int ShopId { get; set; }
    
    public User Author { get; set; }
    public int AuthorId { get; set; }
}