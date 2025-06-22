using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{

    public static Guid? GetUserId(this ClaimsPrincipal? principal)
    {
        // Try the standard JWT subject claim first
        // cuz it gets changed internally by jwt provider
        string? userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                        principal?.FindFirstValue(ClaimTypes.NameIdentifier);


        if (string.IsNullOrEmpty(userId))
        {

            return null;
        }

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            null;
    }
}


