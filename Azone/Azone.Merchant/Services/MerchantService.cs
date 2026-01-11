using Azone.Contracts.Models.Generated;
using Azone.Infra.Common.Models;
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
        var shop = await FindShop(request.Id, false);
        ThrowWithNotFoundIfShopIsNull(shop);
        return shop.ToShopData();
    }

    public override async Task<ShopPreview> GetPreviewShopById(ShopId request, ServerCallContext context)
    {
        var shop = await FindShop(request.Id);
        ThrowWithNotFoundIfShopIsNull(shop);

        return shop.ToShopPreview();
    }

    public override async Task<IsSuccess> UserIsAdminOfShop(OwnersActionData request, ServerCallContext context)
    {
        var shop = await FindShop(request.ShopId.Id, false);
        
        ThrowWithNotFoundIfShopIsNull(shop);
        
        return shop.IsOwner(request.UserId.Id).ToIsSuccess();
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
        
        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditShopDescription(EditShopFieldRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        ThrowWithNotFoundIfShopIsNull(shop);
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = shop.EditDescription(request.NewFieldValue, senderId);
        
        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditShopLogo(EditShopFieldRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        
        ThrowWithNotFoundIfShopIsNull(shop);
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = shop.EditLogoUrl(request.NewFieldValue, senderId);
        
        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditOwnerPermissions(EditOwnerPermissionList request, ServerCallContext context)
    {
        var shop = await LoadShopWithOwnersOrThrow(request.ShopId.Id);

        var sender = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = request.PermissionSet
                ? shop.AddPermissionToOwner(request.UserId.Id, sender, request.NewPermission)
            : shop.RemovePermissionFromOwner(request.UserId.Id, sender, request.NewPermission);

        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> UserHavePermission(UserHavePermissionRequest request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id, false);
        ThrowWithNotFoundIfShopIsNull(shop);
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        if(!shop.IsOwner(senderId))
            throw new RpcException(new Status(StatusCode.PermissionDenied, MerchantError.YouNotAOwner.Code()));

        var result = shop.OwnerHavePermission(request.UserId.Id, request.Permission);
        return result.ToIsSuccess();
    }

    public override async Task<IsSuccess> AddOwner(OwnersActionData request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        ThrowWithNotFoundIfShopIsNull(shop);
        
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        
        var result = shop.AddOwner(request.UserId.Id, ShopOwnerRoles.Manager, senderId);
        
        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<Owners> GetOwnerList(ShopId request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.Id, false);

        var returnedOwners = new Owners();
        returnedOwners.Owners_.AddRange(shop.Owners.Select(x => new UserId { Id = x.UserId }).AsEnumerable());
        return returnedOwners;
    }

    public override async Task<PermissionsList> GetPermissionsOfOwner(OwnersActionData request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id, false);

        ThrowWithNotFoundIfShopIsNull(shop);
        
        var permissions = shop.GetOwnerPermissionsList(request.UserId.Id);
        
        ThrowWithInvalidArgumentIfResultUnsuccessful(permissions);
        
        var returnedPermissions = new PermissionsList();
        
        returnedPermissions.Permissions.AddRange(permissions.Value);
        return returnedPermissions;
    }

    public override async Task<ShopDataWithOwners> GetShopDataWithOwners(ShopId request, ServerCallContext context)
    {
        var shop = await FindShop(request.Id, false);
        ThrowWithNotFoundIfShopIsNull(shop);
        return shop.ToShopDataWithOwners();
    }

    public override async Task<IsSuccess> RemoveOwner(OwnersActionData request, ServerCallContext context)
    {
        var shop = await FindShopWithInclude(request.ShopId.Id);
        ThrowWithNotFoundIfShopIsNull(shop);
        var senderId = jwtHelper.ValidateToken(context).GetUserId();
        var result = shop.RemoveOwner(request.UserId.Id, senderId);
        ThrowWithInvalidArgumentIfResultUnsuccessful(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    private void ThrowWithInvalidArgumentIfResultUnsuccessful(Result result)
    {
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
    }
    
    private void ThrowWithInvalidArgumentIfResultUnsuccessful<T>(Result<T> result)
    {
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
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


    private async Task<Shop?> FindShopWithInclude(int id, bool withTracking = true)
    {
        var query = db.Shops.Where(s => s.Id == id);

        if (!withTracking)
            query = query.AsNoTracking();
        
        return await query
            .Include(s => s.Owners)
            .FirstOrDefaultAsync();
    }
    
    private async Task<Shop?> FindShop(int id, bool withTracking = true)
    {
        var query = db.Shops.Where(s => s.Id == id);

        if (!withTracking)
            query = query.AsNoTracking();
        
        return await query.FirstOrDefaultAsync();
    }
}