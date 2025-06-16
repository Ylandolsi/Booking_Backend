using Microsoft.AspNetCore.Http;
using Application.Abstractions.Authentication;
using Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
namespace Infrastructure.Authentication;



internal class TokenWriterCookies(IHttpContextAccessor httpContextAccessor,
                                  IOptions<JwtOptions>jwtOptions , 
                                  ILogger<TokenWriterCookies> logger) :  ITokenWriterCookies
{

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;



    public void ClearRefreshTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token",
                                                    CreateRefreshCookieOptions(_jwtOptions));

    public void WriteRefreshTokenAsHttpOnlyCookie(string token)
    {

        httpContextAccessor.HttpContext!.Response.Cookies.Append("refresh_token",
                                                                 token,
                                                                 CreateRefreshCookieOptions(_jwtOptions));
        logger.LogInformation("Refresh token written to HTTP-only cookie.");
    }
    private CookieOptions CreateRefreshCookieOptions(JwtOptions jwtAuthOptions)
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

