using Application.Abstractions.Authentication;
using Application.Options;
using Domain.Users.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<JwtOptions> jwtOptions) : ITokenProvider
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public string GenrateJwtToken(User user)
    {
        string secretKey = _jwtOptions.Secret ?? throw new InvalidOperationException("Secret key is not configured in JWT options.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                // dont add too claims 
                // because it will increase the size of the token
                // && 
                // should be safe (not sensitive data)
                // && 
                // if one claims changes frequently , 
                // the client would only get the updated information when the token is refreshed
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer ?? throw new InvalidOperationException("Issuer is not configuterd in JWT "),
            Audience = _jwtOptions.Audience ?? throw new InvalidOperationException("Audience is not configuterd in JWT "),
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }



    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
