using Azone.Contracts.Models.Generated;
using Azone.Merchant.Models.Enums;

public class ShopOwner
{
    public int ShopId { get; private set; }
    public int UserId { get; private set; }

    public ShopOwnerRoles Role { get; private set; }
    public HashSet<Permissions> Permissions { get; private set; }
    public bool IsProtected { get; private set; }

    private ShopOwner() { } // EF

    public ShopOwner(int shopId, int userId, ShopOwnerRoles role, HashSet<Permissions> permissions, bool isProtected = false)
    {
        ShopId = shopId;
        UserId = userId;
        Role = role;
        Permissions = permissions;
        IsProtected = isProtected;
    }
}