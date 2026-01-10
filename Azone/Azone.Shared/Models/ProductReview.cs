namespace Azone.Shared.Models;

public class ProductReview
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Item Item { get; set; }
    public int ProductId { get; set; }
    
    public User Author { get; set; }
    public int AuthorId { get; set; }
}