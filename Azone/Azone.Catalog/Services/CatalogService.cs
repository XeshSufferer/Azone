using Azone.Catalog.DB;
using Azone.Catalog.Models;
using Azone.Catalog.Utils;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Shared;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Azone.Catalog.Services;

public class CatalogService(CatalogDbContext db, Merchant.MerchantClient merchantClient) : Contracts.Models.Generated.Catalog.CatalogBase
{
    public override async Task<IsSuccess> AddProduct(ProductAddRequest request, ServerCallContext context)
    {
        var product = Product.Create(
            request.Logos.AsEnumerable(),
            request.ShopId.Id,
            request.Price.FromProtoDecimal(),
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
        
        productsList.Products.AddRange(products.Select(x => x.ToProductData()));
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
        
        productsList.Products.AddRange(products.Select(x => x.ToProductData()));
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

    public override Task<IsSuccess> EditProductImages(EditProductFieldRequest request, ServerCallContext context)
    {
        return base.EditProductImages(request, context);
    }

    public override Task<IsSuccess> EditProductDescription(EditProductFieldRequest request, ServerCallContext context)
    {
        return base.EditProductDescription(request, context);
    }

    public override Task<IsSuccess> EditProductName(EditProductFieldRequest request, ServerCallContext context)
    {
        return base.EditProductName(request, context);
    }

    public override Task<IsSuccess> EditProductPrice(EditProductFieldRequest request, ServerCallContext context)
    {
        return base.EditProductPrice(request, context);
    }
}