using Microsoft.AspNetCore.Http;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Application.Options;
namespace Infrastructure.Authentication;



internal class TokenWriterCookies(IHttpContextAccessor httpContextAccessor,
                                  IOptions<JwtOptions> jwtOptions,
                                  ILogger<TokenWriterCookies> logger) : ITokenWriterCookies
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public void ClearRefreshTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token",
                                                    CreateRefreshCookieOptions(_jwtOptions));


    public void ClearAccessTokenCookie() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token",
                                                CreateAccessCookieOptions(_jwtOptions));

    public void WriteRefreshTokenAsHttpOnlyCookie(string token)
    {
        httpContextAccessor.HttpContext!.Response.Cookies.Append("refresh_token",
                                                                 token,
                                                                 CreateRefreshCookieOptions(_jwtOptions));
        logger.LogInformation("Refresh token written to HTTP-only cookie.");
    }

    public void WriteAccessTokenAsHttpOnlyCookie(string token)
    {
        httpContextAccessor.HttpContext!.Response.Cookies.Append("access_token",
                                                                 token,
                                                                 CreateAccessCookieOptions(_jwtOptions));
        logger.LogInformation("Access token written to HTTP-only cookie.");
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

    private CookieOptions CreateAccessCookieOptions(AccessOptions jwtAuthOptions)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // TODO : change to true 
            Path = "/",
            SameSite = SameSiteMode.Lax, // TODO : change to strict 
            Expires = DateTime.UtcNow.AddMinutes(jwtAuthOptions.ExpirationInMinutes)
        };
    }
    // TODO : check which is more secure LAX or other option 
    // * and set secure = true 
    //     2. **SameSite Attribute**:
    //    - Adding the `SameSite` attribute to the cookie can help mitigate CSRF attacks:
    //      - `SameSite=Strict`: The cookie is only sent with requests originating from the same site.
    //      - `SameSite=Lax`: The cookie is sent with top-level navigation and GET requests but not with cross-origin POST requests.
    //      - `SameSite=None; Secure`: The cookie is sent with cross-origin requests but only over HTTPS.
    //-------------------------
    // WHEN SameSite=  strict is not suitable and we do need a CSRF protection using tokens (antiforgery tokens )
    // https://duendesoftware.com/blog/20250325-understanding-antiforgery-in-aspnetcore
    // 1. **Cross-Origin Requests Are Required**:
    //    - If your application needs to support cross-origin requests(e.g., if your frontend and backend are hosted on different domains), you cannot use `SameSite=Strict`. Instead, you would need to use `SameSite=None; Secure` and implement CSRF protection using tokens.

    // 2. ** Third-Party Integrations**:
    //    - If your application integrates with third-party services that require cross-origin requests, `SameSite=Strict` will block those requests, and you may need to rely on CSRF tokens instead.


    // anti forgery tokens protection in brief: 
    // when the user visits a website , the server generates a unique anti forgery tokens
    // this token is sent to the user's browser and embedded in the webpage ( as hidden form field or in a custom header )
    // when the user submits a form or makes a request , the token is sent back to the server 
    // and itself validates the token by comparing it with the one it generated 
}