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
            ShopLogoUrl = shop.LogoUrl,
        };

    public static ShopPreview ToShopPreview(this Shop shop)
        => new ShopPreview
        {
            LogoUrl = shop.LogoUrl,
            Name = shop.Name,
        };

    public static ShopDataWithOwners ToShopDataWithOwners(this Shop shop)
    {
        var owners = new Owners();
        owners.Owners_.AddRange(shop.Owners.Select(x => new UserId { Id = x.UserId }).ToList());

        return new ShopDataWithOwners
        {
            Owners = owners,
            Id = shop.Id,
            ShopName = shop.Name,
            ShopDescription = shop.Description,
            ShopLogoUrl = shop.LogoUrl,
        };
    }
    
    public static IsSuccess ToIsSuccess(this bool isSuccess)
        => new IsSuccess { Success = isSuccess };
}