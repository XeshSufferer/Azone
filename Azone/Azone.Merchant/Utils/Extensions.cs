using Azone.Contracts.Models.Generated;
using Azone.Merchant.Models;
using Azone.Merchant.Models.Enums;

namespace Azone.Merchant.Utils;

public static class Extensions
{
    public static string Code(this MerchantError error)
        => $"Merchant.{error.ToString()}";


    public static ShopData ToShopData(this Shop shop)
        => new ShopData
        {
            Id = shop.Id,
            ShopName = shop.Name,
            ShopDescription = shop.Description,
        };
}