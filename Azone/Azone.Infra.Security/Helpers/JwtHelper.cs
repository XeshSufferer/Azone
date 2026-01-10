using Grpc.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azone.Infra.Security.DataObjects;

namespace Azone.Infra.Security.Helpers;

public class JwtHelper : IJwtHelper
{
    private readonly JwtOptions _options;

    public JwtHelper(JwtOptions options)
    {
        _options = options;
    }

    public ClaimsPrincipal? ValidateToken(ServerCallContext context)
    {
        var authHeader = context.RequestHeaders.GetValue("authorization");
        if (string.IsNullOrEmpty(authHeader) || 
            !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var jwt = authHeader["Bearer ".Length..];
        return ValidateToken(jwt, _options.Issuer, _options.Audience, _options.Key);
    }

    public ClaimsPrincipal? ValidateToken(
        string jwtToken, 
        string issuer, 
        string audience, 
        string signingKey)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(signingKey);

        try
        {
            var principal = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}