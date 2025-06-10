using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Authentication;

public interface ITokenWriterCookies
{
    void WriteAccessTokenAsHttpOnlyCookie(string token);
    void ClearAccessTokenCookie();
    void WriteRefreshTokenAsHttpOnlyCookie(string token);
    void ClearRefreshTokenCookie(); 

}