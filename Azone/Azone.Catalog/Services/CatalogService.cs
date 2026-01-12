using Azone.Catalog.DB;
using Azone.Catalog.Models;
using Azone.Catalog.Models.Enums;
using Azone.Catalog.Utils;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Common.Models;
using Azone.Infra.Security.Helpers;
using Azone.Infra.Shared;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Azone.Catalog.Services;

public class CatalogService(CatalogDbContext db,
    Merchant.MerchantClient merchantClient, IJwtHelper jwtHelper) : Contracts.Models.Generated.Catalog.CatalogBase
{
    public override async Task<IsSuccess> AddProduct(ProductAddRequest request, ServerCallContext context)
    {
        var product = Product.Create(
            request.Logos.AsEnumerable(),
            request.ShopId.Id,
            request.Price.ToDecimal(),
            request.Name,
            request.Description);
        
        await db.Products.AddAsync(product);
        await db.SaveChangesAsync();
        return true.ToIsSuccess();
    }

    public override async Task<ProductsList> GetProducts(GetProductRequest request, ServerCallContext context)
    {
        var productsList = new  ProductsList();
        
        var products = await db.Products.OrderByDescending(p => p.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Count)
            .AsNoTracking()
            .ToListAsync();
        
        productsList.Products.AddRange(products.Select(x => x.ToOwnerActionData()));
        return productsList;
    }

    public override async Task<ProductsList> GetShopProduct(GetProductFromShopRequest request, ServerCallContext context)
    {
        var productsList = new  ProductsList();
        
        var products = await db.Products.Where(p => p.ShopId == request.ShopId.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Count)
            .AsNoTracking()
            .ToListAsync();
        
        productsList.Products.AddRange(products.Select(x => x.ToOwnerActionData()));
        return productsList;
    }

    public override async Task<IsSuccess> RemoveProduct(ProductId request, ServerCallContext context)
    {
        var productForDelete = new Product
        {
            Id = request.Id,
        };
        
        db.Products.Remove(productForDelete);
        await db.SaveChangesAsync();
        return true.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditProductImages(EditProductFieldRequest request, ServerCallContext context)
    {
        var userid = jwtHelper.ValidateToken(context).GetUserId();
        var permissions = await merchantClient.OwnerCanEditProductImagesAsync(request.ToOwnerActionData(userid));
        CheckPermission(permissions);
        
        var product = await FindProductWithInclude(request.Id.Id);

        var result = product.AddImage(new LogoUrl(request.NewFieldValue, product.ShopId));
        ThrowWithInvalidArgumentIfResultUnsuccess(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditProductDescription(EditProductFieldRequest request, ServerCallContext context)
    {
        var userid = jwtHelper.ValidateToken(context).GetUserId();
        var permissions = await merchantClient.OwnerCanEditProductDescriptionAsync(request.ToOwnerActionData(userid));
        
        CheckPermission(permissions);
        
        var product = await FindProduct(request.Id.Id);
        var result = product.EditDescription(request.NewFieldValue);
        ThrowWithInvalidArgumentIfResultUnsuccess(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditProductName(EditProductFieldRequest request, ServerCallContext context)
    {
        var userid = jwtHelper.ValidateToken(context).GetUserId();
        var permissions = await merchantClient.OwnerCanEditProductNameAsync(request.ToOwnerActionData(userid));
        CheckPermission(permissions);
        
        var product = await FindProduct(request.Id.Id);
        
        var result = product.EditName(request.NewFieldValue);
        ThrowWithInvalidArgumentIfResultUnsuccess(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    public override async Task<IsSuccess> EditProductPrice(EditPriceFieldRequest request, ServerCallContext context)
    {
        var userid =  jwtHelper.ValidateToken(context).GetUserId();
        var permissions = await merchantClient.OwnerCanEditProductPriceAsync(request.ToOwnerActionData(userid));
        CheckPermission(permissions);
        
        var product = await FindProduct(request.Id.Id);
        var result = product.EditPrice(request.NewPrice.ToDecimal());
        ThrowWithInvalidArgumentIfResultUnsuccess(result);
        await db.SaveChangesAsync();
        return result.IsSuccess.ToIsSuccess();
    }

    private async Task<Product?> FindProductWithInclude(int id, bool withTracking = true)
    {
        var query = db.Products
            .Include(p => p.ImageUrls)
            .Where(p => p.Id == id);
        
        if (!withTracking)
            query = query.AsTracking();
        
        return await query.FirstOrDefaultAsync();
    }   

    private async Task<Product?> FindProduct(int id, bool withTracking = true)
    {
        var query = db.Products
            .Where(p => p.Id == id);
        
        if (!withTracking)
            query = query.AsTracking();
        
        return await query.FirstOrDefaultAsync();
    }

    private void ThrowWithInvalidArgumentIfResultUnsuccess(Result result)
    {
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));
    }
    
    private void CheckPermission(IsSuccess result)
    {
        if(!result.Success)
            throw new RpcException(new Status(StatusCode.PermissionDenied, CatalogError.YouDontHavePermissionForThisAction.Code()));
    }
}