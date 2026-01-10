using Azone.Shared.Models.Enums;

namespace Azone.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public UserRole UserRole { get; set; }
    
    public ICollection<Shop> Shops { get; set; }
    public ICollection<CartItem> Cart { get; set; }
    public ICollection<Order> Orders { get; set; }
    
}