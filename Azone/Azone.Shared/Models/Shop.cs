namespace Azone.Shared.Models;

public class Shop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Item> Products { get; set; }
    public ICollection<ShopReview> Reviews { get; set; }
    public ICollection<User> Admins { get; set; }
}