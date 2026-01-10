using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azone.Accounts.Services.Sub_Services.Contracts;
using Azone.Infra.Security.DataObjects;
using Microsoft.IdentityModel.Tokens;

namespace Azone.Accounts.Services.Sub_Services;

public class JwtService : IJwtService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly int _lifetimeMinutes;

    public JwtService(JwtOptions options)
    {
        _issuer = options.Issuer ?? throw new ArgumentNullException(nameof(options.Issuer));
        _audience = options.Audience ?? throw new ArgumentNullException(nameof(options.Audience));
        _lifetimeMinutes = options.ExpireMinutes;

        if (string.IsNullOrEmpty(options.Key) || options.Key.Length < 32)
            throw new ArgumentException("Signing key must be at least 32 characters long.", nameof(options.Key));

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
    }

    public string IssueToken(int userId, IEnumerable<string>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(userId.ToString()))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_lifetimeMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}