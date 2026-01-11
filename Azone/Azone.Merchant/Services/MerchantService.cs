using Azone.Contracts.Models.Generated;
using Azone.Infra.Security.Helpers;
using Azone.Merchant.DBs;
using Azone.Merchant.Models;
using Azone.Merchant.Models.Enums;
using Azone.Merchant.Utils;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Azone.Merchant.Services;

public class MerchantService(ILogger<MerchantService> logger, MerchantDbContext db, IJwtHelper jwtHelper) : Contracts.Models.Generated.Merchant.MerchantBase
{
    public override async Task<ShopData> CreateShop(CreateShopRequest request, ServerCallContext context)
    {
        // ТОКЕН ВАЛИДИРУЕТСЯ СООТВЕТСТВУЯ ПАРАМЕТРАМ БЕЗОПАСНОСТИ НА GATEWAY УРОВНЕ!!!
        var token = jwtHelper.ValidateToken(context);

        var shop = Shop.Create(request.ShopName, token.GetUserId());
        
        await db.Shops.AddAsync(shop);
        await db.SaveChangesAsync();

        logger.LogInformation("Created new shop {ShopId}", shop.Id);
        
        return shop.ToShopData();
    }

    public override async Task<ShopData> GetShopById(ShopId request, ServerCallContext context)
    {
        var shop = (await db.Shops.FirstOrDefaultAsync(s => s.Id == request.Id));
        ThrowWithNotFoundIfShopIsNull(shop);
        return shop.ToShopData();
    }

    public override async Task<IsSuccess> UserIsAdminOfShop(UserIsAdminOfShopRequest request, ServerCallContext context)
    {
        var shop = await FindShop(request.ShopId.Id);
        
        ThrowWithNotFoundIfShopIsNull(shop);
        
        return new IsSuccess { Success = shop.IsOwner(request.UserId.Id) };
    }

    public override Task<PermissionsList> GetPermissionList(Empty request, ServerCallContext context)
    {
        var reply = new PermissionsList();
        reply.Permissions.AddRange(PermissionSet.All);
        return Task.FromResult(reply);
    }

    public override async Task<IsSuccess> EditShopName(EditShopFieldRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);

        if (shop == null)
            throw new RpcException(new Status(StatusCode.NotFound, MerchantError.ShopNotFound.Code()));
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();

        var result = shop.EditName(request.NewFieldValue, senderId);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
        
        await db.SaveChangesAsync();
        return new IsSuccess { Success = true };
    }

    public override async Task<IsSuccess> EditShopDescription(EditShopFieldRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        ThrowWithNotFoundIfShopIsNull(shop);
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = shop.EditDescription(request.NewFieldValue, senderId);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
        
        await db.SaveChangesAsync();
        return new IsSuccess { Success = true };
    }

    public override async Task<IsSuccess> EditShopLogo(EditShopFieldRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        
        ThrowWithNotFoundIfShopIsNull(shop);
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = shop.EditLogoUrl(request.NewFieldValue, senderId);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
        
        await db.SaveChangesAsync();
        return new IsSuccess { Success = true };
    }

    public override async Task<IsSuccess> EditOwnerPermissions(EditOwnerPermissionList request, ServerCallContext context)
    {
        var shop = await LoadShopWithOwnersOrThrow(request.ShopId.Id);

        var sender = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = request.PermissionSet
                ? shop.AddPermissionToOwner(request.UserId.Id, sender, request.NewPermission)
            : shop.RemovePermissionFromOwner(request.UserId.Id, sender, request.NewPermission);

        if (!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));

        await db.SaveChangesAsync();
        return new IsSuccess { Success = true };
    }

    private void ThrowWithNotFoundIfShopIsNull(Shop shop)
    {
        if(shop == null)
            throw new RpcException(new Status(StatusCode.NotFound, MerchantError.ShopNotFound.Code()));
    }
    
    private async Task<Shop> LoadShopWithOwnersOrThrow(int id)
    {
        var shop = await FindShopWithInclude(id);
        if (shop == null)
            throw new RpcException(new Status(StatusCode.NotFound, MerchantError.ShopNotFound.Code()));
        return shop;
    }


    private async Task<Shop?> FindShopWithInclude(int id) =>
        await db.Shops
            .Include(s => s.Owners)
            .FirstOrDefaultAsync(s => s.Id == id);
    
    private async Task<Shop?> FindShop(int id) =>
        await db.Shops
            .FirstOrDefaultAsync(s => s.Id == id);
}