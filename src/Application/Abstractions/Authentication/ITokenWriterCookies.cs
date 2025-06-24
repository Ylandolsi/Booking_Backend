using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Authentication;

public interface ITokenWriterCookies
{
    void WriteRefreshTokenAsHttpOnlyCookie(string token);
    void ClearRefreshTokenCookie();

    void WriteAccessTokenAsHttpOnlyCookie(string token);

    void ClearAccessTokenCookie();

}