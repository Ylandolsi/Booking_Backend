namespace Application.Options;

public sealed class JwtOptions
{
    public const string JwtOptionsKey = "Jwt"; // ## name in appsettings.json
    public AccessOptions AccessToken { get; set; } = new AccessOptions();

}

public class JwtSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInMinutes { get; set; }



}
public sealed class AccessOptions : JwtSettings
{
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public int MaxActiveTokensPerUser { get; set; } = 5;

    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}

