using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeviceTrust.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("No user ID claim found on the current principal.");
    }
}