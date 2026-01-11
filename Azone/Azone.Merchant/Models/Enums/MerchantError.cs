namespace Azone.Merchant.Models.Enums;

public enum MerchantError
{
    UserIsNotShopOwner,
    ShopHaveMinOneOwner,
    YouNotAOwner,
    ThisUserIsNotOwner,
    YouPermissionLevelIsTooLow,
    NewShopLogoUrlIsInvalid,
    ShopNotFound,
    ThisUserIsProtected,
    ThisUserDontHaveThisPermission,
}