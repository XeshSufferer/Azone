using System.Security.Claims;
using Grpc.Core;

namespace Azone.Infra.Security.Helpers;

public interface IJwtHelper
{
    ClaimsPrincipal? ValidateToken(ServerCallContext context);

    ClaimsPrincipal? ValidateToken(
        string jwtToken,
        string issuer,
        string audience,
        string signingKey);
}