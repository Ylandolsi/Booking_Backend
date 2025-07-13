
//using Application.Options;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.JsonWebTokens;
//using Microsoft.IdentityModel.Tokens;
//using System.Security.Claims;
//using System.Security.Cryptography;

//public interface IJwtValidationService
//{
//    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
//    Task<bool> IsTokenBlacklistedAsync(string jti);
//    Task BlacklistTokenAsync(string jti, DateTime expiry);
//}

//public class JwtValidationService(IOptions<JwtOptions> jwtOptions) : IJwtValidationService
//{
//    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

//    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
//    {
//        try
//        {
//            if (string.IsNullOrEmpty(_jwtOptions.PublicKey))
//                throw new InvalidOperationException("Public key is not configured.");



//            var rsa = RSA.Create();




//            var publicKey = _jwtOptions.PublicKey
//                            .Replace("\\n", "\n")
//                            .Trim();

//            rsa.ImportFromPem(publicKey.ToCharArray());

//            var securityKey = new RsaSecurityKey(rsa);


//            var tokenHandler = new JsonWebTokenHandler();
//            var validationParameters = new TokenValidationParameters
//            {
//                ValidateIssuerSigningKey = true,
//                IssuerSigningKey = securityKey,
//                ValidateIssuer = true,
//                ValidIssuer = _jwtOptions.Issuer,
//                ValidateAudience = true,
//                ValidAudience = _jwtOptions.Audience,
//                ValidateLifetime = true,
//                ValidateTokenReplay = false,
//                ClockSkew = TimeSpan.FromSeconds(30), 
//                RequireSignedTokens = true,
//                RequireExpirationTime = true
//            };

//            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

//            if (!result.IsValid)
//                return null;

//            // Check if token is blacklisted
//            var jtiClaim = result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti);
//            if (jtiClaim != null && await IsTokenBlacklistedAsync(jtiClaim.Value))
//                return null;

//            return new ClaimsPrincipal(result.ClaimsIdentity);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Token validation failed: {ex.Message}");
//            return null;
//        }
//    }

//    public async Task<bool> IsTokenBlacklistedAsync(string jti)
//    {
//        // Implement with Redis or database
//        // For now, return false (no blacklisting)
//        await Task.CompletedTask;
//        return false;
//    }

//    public async Task BlacklistTokenAsync(string jti, DateTime expiry)
//    {
//        // Implement with Redis or database
//        // Store jti with TTL until token expiry
//        await Task.CompletedTask;
//    }
//}