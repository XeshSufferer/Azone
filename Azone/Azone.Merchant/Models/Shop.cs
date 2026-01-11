using Azone.Contracts.Models.Generated;
using Azone.Infra.Common.Models;
using Azone.Merchant.Models.Enums;
using Azone.Merchant.Models.Records;
using Azone.Merchant.Utils;

namespace Azone.Merchant.Models;

public class Shop
{
    
    public int Id { get; init; }
    
    private string _name { get; set; }
    private string _description { get; set; }
    private string _logoUrl { get; set; }
    
    public string Name { get => _name; init => _name = value; }
    public string Description { get => _description; init => _description = value; }
    public string LogoUrl { get => _logoUrl; init => _logoUrl = value; }

    
    
    private readonly HashSet<ShopOwner> _owners = new();
    public IReadOnlyCollection<ShopOwner> Owners => _owners;
    
    
    
    // Backing Fields
    // For EF Core
    protected HashSet<ShopOwner> OwnersBackingField => _owners;
    
    public static Shop Create(string name, int creatorUserId)
    {
        var shop = new Shop
        {
            _name = name,
        };
        shop._owners.Add(new ShopOwner(shop.Id, creatorUserId,
            ShopOwnerRoles.Owner,
            PermissionSet.All));
        return shop;
    }

    public Result SetLogoUrl(string url, int invokerId)
    {
        var invoker = FindOwner(invokerId);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(!invoker.Permissions.Contains(Permissions.EditLogo))
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        _logoUrl = url;
        return Result.Success();
    }

    public Result EditName(string newName, int id)
    {
        var invoker = FindOwner(id);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(!invoker.Permissions.Contains(Permissions.EditName))
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        _name = newName;
        return Result.Success();
    }

    public Result EditDescription(string newDescription, int invokerId)
    {
        var invoker = FindOwner(invokerId);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(!invoker.Permissions.Contains(Permissions.EditDescriptions))
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        _description = newDescription;
        return Result.Success();
    }

    public Result EditLogoUrl(string url, int invokerId)
    {
        if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return Result.Failure(MerchantError.NewShopLogoUrlIsInvalid.Code());
        
        var invoker = FindOwner(invokerId);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(!invoker.Permissions.Contains(Permissions.EditLogo))
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        _logoUrl = url;
        return Result.Success();
    }

    public Result AddOwner(int id, ShopOwnerRoles roleForNewOwner, int invokerId)
    {
        var invoker = FindOwner(invokerId);
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(invoker.Role <  roleForNewOwner)
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        if(_owners.Any(x => x.UserId == id))
            return Result.Failure(MerchantError.ThisUserIsNotOwner.Code());
        
        _owners.Add(new ShopOwner(Id, id, roleForNewOwner, []));
        return Result.Success();
    }

    public Result RemoveOwner(int id, int invokerId)
    {
        var invoker = FindOwner(invokerId);
        var ownerForRemove = FindOwner(id);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(ownerForRemove == null)
            return Result.Failure(MerchantError.ThisUserIsNotOwner.Code());

        if (!invoker.Permissions.Contains(Permissions.EditOwnersList))
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());    
        
        if(ownerForRemove.IsProtected && !invoker.Permissions.Contains(Permissions.EditProtectedOwners))
            return Result.Failure(MerchantError.ThisUserIsProtected.Code());
        
        if (ownerForRemove == null)
            return Result.Failure(MerchantError.UserIsNotShopOwner.Code());
        
        if(_owners.Count < 2)
            return Result.Failure(MerchantError.ShopHaveMinOneOwner.Code());
        
        
        
        _owners.RemoveWhere(x => x.UserId == id);
        return Result.Success();
    }

    public bool IsOwner(int id)
    {
        return _owners.Any(x => x.UserId == id);
    }

    public Result<ShopOwner> GetOwner(int id)
    {
        var owner = FindOwner(id);
        if(owner == null)
            return Result<ShopOwner>.Failure(MerchantError.UserIsNotShopOwner.Code());
        
        return Result<ShopOwner>.Success(owner);
    }
    
    public bool OwnerHavePermission(int id, Permissions permission)
    {
        var owner = FindOwner(id);
        if (owner == null)
            return false;
        
        return owner.Permissions.Contains(permission);
    }

    public Result AddPermissionToOwner(int id, int invokerId, Permissions permission)
    {
        var ownerForAddPerm = FindOwner(id);
        
        if(ownerForAddPerm == null)
            return Result.Failure(MerchantError.UserIsNotShopOwner.Code());
        
        var invoker = FindOwner(invokerId);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());

        var invokerCanEditProtectedUsers = invoker.Permissions.Contains(Permissions.EditProtectedOwners);
        
        if(ownerForAddPerm.IsProtected && !invokerCanEditProtectedUsers)
            return Result.Failure(MerchantError.ThisUserIsProtected.Code());
        
        if (!invoker.Permissions.Contains(Permissions.EditRolesPermissions) && !invokerCanEditProtectedUsers)
            return Result.Failure(MerchantError.YouPermissionLevelIsTooLow.Code());
        
        ownerForAddPerm.Permissions.Add(permission);
        return Result.Success();
    }

    public Result RemovePermissionFromOwner(int id, int invokerId, Permissions permission)
    {
        var invoker = FindOwner(invokerId);
        var ownerForRemove = FindOwner(id);
        
        if(invoker == null)
            return Result.Failure(MerchantError.YouNotAOwner.Code());
        
        if(ownerForRemove == null)
            return Result.Failure(MerchantError.ThisUserIsNotOwner.Code());
        
        if(!ownerForRemove.Permissions.Contains(permission))
            return Result.Failure(MerchantError.ThisUserDontHaveThisPermission.Code());
        
        if(_owners.Count < 2 && ownerForRemove.IsProtected)
            return Result.Failure(MerchantError.ShopHaveMinOneOwner.Code());
        
        if(ownerForRemove.IsProtected && !invoker.Permissions.Contains(Permissions.EditProtectedOwners))
            return Result.Failure(MerchantError.ThisUserIsProtected.Code());
        
        ownerForRemove.Permissions.Remove(permission);
        return Result.Success();
    }

    private ShopOwner? FindOwner(int userId)
        => _owners.FirstOrDefault(x => x.UserId ==  userId);

    
}