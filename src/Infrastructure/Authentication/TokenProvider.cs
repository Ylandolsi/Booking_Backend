using Application.Abstractions.Authentication;
using Application.Options;
using Domain.Users.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<JwtOptions> jwtOptions) : ITokenProvider
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public string GenerateJwtToken(User user)
    {
        if (string.IsNullOrEmpty(_jwtOptions.PrivateKey))
            throw new InvalidOperationException("Private key is not configured in JWT options.");

        if (string.IsNullOrEmpty(_jwtOptions.Issuer))
            throw new InvalidOperationException("Issuer is not configured in JWT options.");

        if (string.IsNullOrEmpty(_jwtOptions.Audience))
            throw new InvalidOperationException("Audience is not configured in JWT options.");

        var rsa = RSA.Create();

        try
        {
            var privateKey = _jwtOptions.PrivateKey
                .Replace("\\n", "\n")
                .Trim();

            rsa.ImportFromPem(privateKey.ToCharArray());

            var securityKey = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,

            };

            var handler = new JsonWebTokenHandler();
            string token = handler.CreateToken(tokenDescriptor);
            return token;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error generating JWT token: {e.Message}");

        }
        return String.Empty;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}