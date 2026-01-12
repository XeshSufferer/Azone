using Azone.Catalog.Models;
using Azone.Catalog.Models.Enums;
using Azone.Contracts.Models.Generated;

namespace Azone.Catalog.Utils;

public static class Extensions
{
    public static string Code(this CatalogError error)
        => $"Catalog.{error.ToString()}";

    public static decimal ToDecimal(this ProtoDecimal protoDecimal)
    {
        if (protoDecimal.Scale < 0 || protoDecimal.Scale > 28)
            throw new ArgumentOutOfRangeException(nameof(protoDecimal.Scale), "Scale must be between 0 and 28 for C# decimal.");

        try
        {
            var value = protoDecimal.Value;
            bool isNegative = value < 0;
            ulong absValue = (ulong)Math.Abs(value);

            int lo = (int)(absValue & 0xFFFFFFFFUL);
            int mid = (int)((absValue >> 32) & 0xFFFFFFFFUL);
            int hi = 0;

            return new decimal(lo, mid, hi, isNegative, (byte)protoDecimal.Scale);
        }
        catch (OverflowException)
        {
            throw new OverflowException($"Value {protoDecimal.Value} with scale {protoDecimal.Scale} cannot be represented as a C# decimal.");
        }
    }

    public static ProtoDecimal ToProtoDecimal(this decimal value)
    {
        int[] bits = decimal.GetBits(value);
    
        bool isNegative = (bits[3] & 0x80000000) != 0;
        byte scale = (byte)((bits[3] >> 16) & 0x7F);

        ulong mantissa = ((ulong)(uint)bits[2] << 64) |
                         ((ulong)(uint)bits[1] << 32) |
                         (uint)bits[0];

        if (mantissa > (ulong)long.MaxValue)
            throw new OverflowException("Decimal magnitude too large to fit in int64-based proto Decimal.");

        long intValue = (long)mantissa;
        if (isNegative)
            intValue = -intValue;

        return new ProtoDecimal()
        {
            Value = intValue,
            Scale = scale
        };
    }

    public static ProductData ToOwnerActionData(this Product product)
    {
        var data = new ProductData
        {
            Name = product.Name,
            Description = product.Description
        };
        
        data.Logos.LogoUrls.AddRange(product.ImageUrls.Select(x => x.Url));
        return data;
    }

    public static OwnersActionData ToOwnerActionData(this EditProductFieldRequest request, int userId)
    {
        return new OwnersActionData
        {
            ShopId = request.Id.ShopId,
            UserId = new UserId
            {
                Id = userId
            },
        };
    }
    
    public static OwnersActionData ToOwnerActionData(this EditPriceFieldRequest request, int userId)
    {
        return new OwnersActionData
        {
            ShopId = request.Id.ShopId,
            UserId = new UserId
            {
                Id = userId
            },
        };
    }
}