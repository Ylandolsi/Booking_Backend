using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimsIdentifiers.UserId);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            null;
    }



}
