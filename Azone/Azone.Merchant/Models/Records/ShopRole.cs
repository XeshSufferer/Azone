using Azone.Contracts.Models.Generated;
using Azone.Infra.Common.Models;
using Azone.Merchant.Models.Enums;

namespace Azone.Merchant.Models.Records;

public class ShopRole
{
    public string Name { get; set; }
    public HashSet<Permissions> Permissions { get; set; } = new();
    public bool Protected { get; set; }
    public bool CanAssing { get; set; }
};