//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Security.Claims;
//using System.Text.Encodings.Web;
//using Microsoft.Extensions.Primitives;
//using System.Text.Json;
//namespace IntegrationsTests.Abstractions;

//public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//{
//    public const string AuthenticationScheme = "Test";
//    public const string TestUserClaimsHeader = "X-Test-User-Claims";

//    public TestAuthHandler(
//        IOptionsMonitor<AuthenticationSchemeOptions> options,
//        ILoggerFactory logger,
//        UrlEncoder encoder)
//        : base(options, logger, encoder)
//    {
//    }

//    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//    {
//        if (!Request.Headers.TryGetValue(TestUserClaimsHeader, out StringValues claimsValue))
//        {
//            return Task.FromResult(AuthenticateResult.NoResult());
//        }

//        try
//        {
//            var serializedClaims = JsonSerializer.Deserialize<List<SimpleClaim>>(claimsValue.ToString());

//            if (serializedClaims == null || !serializedClaims.Any())
//            {
//                return Task.FromResult(AuthenticateResult.Fail("Invalid claims format in header."));
//            }

//            var claims = serializedClaims.Select(sc => new Claim(sc.Type, sc.Value)).ToList();
//            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
//            var principal = new ClaimsPrincipal(identity);
//            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

//            return Task.FromResult(AuthenticateResult.Success(ticket));
//        }
//        catch (Exception ex)
//        {
//            return Task.FromResult(AuthenticateResult.Fail($"Error deserializing claims: {ex.Message}"));
//        }
//    }


//    private class SimpleClaim
//    {
//        public string Type { get; set; } = string.Empty;
//        public string Value { get; set; } = string.Empty;
//    }
//}