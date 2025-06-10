using Microsoft.AspNetCore.Http;
using Application.Abstractions.Authentication;
using Application.Options;
using Microsoft.Extensions.Options;
namespace Infrastructure.Authentication;



internal class TokenWriterCookies(IHttpContextAccessor httpContextAccessor,
                                  IOptions<JwtOptions>jwtOptions) :  ITokenWriterCookies
{

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    public void WriteAccessTokenAsHttpOnlyCookie(string token) => httpContextAccessor.HttpContext!.Response.Cookies.Append("access_token",
                                                                 token,
                                                                 CreateAccessCookieOptions(_jwtOptions));


    public void ClearRefreshTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token",
                                                    CreateRefreshCookieOptions(_jwtOptions));

    public void WriteRefreshTokenAsHttpOnlyCookie(string token) => httpContextAccessor.HttpContext!.Response.Cookies.Append("refresh_token",
                                                                 token,
                                                                 CreateRefreshCookieOptions(_jwtOptions));


    public void ClearAccessTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token",
                                                    CreateAccessCookieOptions(_jwtOptions));



    private CookieOptions CreateAccessCookieOptions(JwtOptions jwtAuthOptions) => new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(jwtAuthOptions.ExpirationInMinutes)
    };

    private CookieOptions CreateRefreshCookieOptions(JwtOptions jwtAuthOptions)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(jwtAuthOptions.RefreshTokenExpirationDays)
        };
    }
}

