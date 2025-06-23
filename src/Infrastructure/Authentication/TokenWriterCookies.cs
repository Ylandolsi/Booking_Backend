using Microsoft.AspNetCore.Http;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Application.Options;
namespace Infrastructure.Authentication;



internal class TokenWriterCookies(IHttpContextAccessor httpContextAccessor,
                                  IOptions<JwtOptions>jwtOptions , 
                                  ILogger<TokenWriterCookies> logger) :  ITokenWriterCookies
{

    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;



    public void ClearRefreshTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token",
                                                    CreateRefreshCookieOptions(_jwtOptions));

    public void WriteRefreshTokenAsHttpOnlyCookie(string token)
    {

        httpContextAccessor.HttpContext!.Response.Cookies.Append("refresh_token",
                                                                 token,
                                                                 CreateRefreshCookieOptions(_jwtOptions));
        logger.LogInformation("Refresh token written to HTTP-only cookie.");
    }
    private CookieOptions CreateRefreshCookieOptions(AccessOptions jwtAuthOptions)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            Path = "/",
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(jwtAuthOptions.RefreshTokenExpirationDays)
        }; 
    }
}

