using Azone.Catalog.Models.Enums;
using Azone.Catalog.Utils;
using Azone.Infra.Common.Models;

namespace Azone.Catalog.Models;

public class Product
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    
    
    private string _name { get; set; }
    private string _description { get; set; }
    private HashSet<LogoUrl> _imageUrls { get; set; }
    
    public string Name { get => _name; init => _name = value; }
    public string Description { get => _description; init => _description = value; }
    
    public IReadOnlyCollection<LogoUrl> ImageUrls => _imageUrls;
    
    // Backing Fields
    // For EF Core
    protected HashSet<LogoUrl> OwnersBackingField => _imageUrls;
    
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public static Product Create(IEnumerable<string> urls, 
        int shopId, decimal price,
        string name, string description)
    {
        return new Product
        {
            ShopId = shopId,
            Name = name,
            Description = description,
            Price = price,
            IsActive = true,
            IsDeleted = false,
            _imageUrls = urls.Select(url => new LogoUrl(url, shopId)).ToHashSet(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result EditName(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
            return Result.Failure(CatalogError.ThisFieldCannotBeEmpty.Code());
        
        _name = Name;
        return Result.Success();
    }

    public Result EditDescription(string newDescription)
    {
        if (!string.IsNullOrWhiteSpace(newDescription))
            return Result.Failure(CatalogError.ThisFieldCannotBeEmpty.Code());
        
        _description = newDescription;
        return Result.Success();
    }

    public Result RemoveImage(LogoUrl imageUrl)
    {
        if (!_imageUrls.Contains(imageUrl))
            return Result.Failure(CatalogError.ThisFieldDoesNotExist.Code());
        
        if(Uri.IsWellFormedUriString(imageUrl.Url, UriKind.Absolute))
            return Result.Failure(CatalogError.InvalidImageUrl.Code());
        
        _imageUrls.Remove(imageUrl);
        return Result.Success();
    }

    public Result AddImage(LogoUrl imageUrl)
    {
        if (_imageUrls.Contains(imageUrl))
            return Result.Failure(CatalogError.ThisFieldDoesExist.Code());
        
        if(Uri.IsWellFormedUriString(imageUrl.Url, UriKind.Absolute))
            return Result.Failure(CatalogError.InvalidImageUrl.Code());
        
        _imageUrls.Add(imageUrl);
        return Result.Success();
    }

    public Result EditPrice(decimal newPrice)
    {
        if (newPrice < 0)
            return Result.Failure(CatalogError.PriceCannotBeNegative.Code());
        
        if(newPrice == 0)
            return Result.Failure(CatalogError.PriceCannotBeZero.Code());
        
        Price = newPrice;
        return Result.Success();
    }
    
}