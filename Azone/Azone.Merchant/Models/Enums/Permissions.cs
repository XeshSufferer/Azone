using Azone.Contracts.Models.Generated;

namespace Azone.Merchant.Models.Enums;

public static class PermissionSet
{
    public static readonly HashSet<Permissions> All = 
        Enum.GetValues<Permissions>().ToHashSet();
}