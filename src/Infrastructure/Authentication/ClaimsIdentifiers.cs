using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Authentication;

internal static class ClaimsIdentifiers {

    public const string IsEmailVerified = "IsEmailVerified"; 
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string UserId = JwtRegisteredClaimNames.Sub; 
}
