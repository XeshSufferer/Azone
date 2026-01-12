namespace Azone.Catalog.Models.Enums;

public enum CatalogError
{
    YouDontHavePermissionForThisAction,
    ThisFieldCannotBeEmpty,
    ThisFieldDoesExist,
    ThisFieldDoesNotExist,
    InvalidImageUrl,
    PriceCannotBeNegative,
    PriceCannotBeZero,
}