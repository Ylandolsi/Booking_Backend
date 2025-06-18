using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;

namespace IntegrationsTests.Abstractions;

public abstract class AuthenticationTestBase : BaseIntegrationTest
{
    protected AuthenticationTestBase(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    protected async Task RegisterAndVerifyUser(string email, string password, bool verify = true)
    {
        var registrationPayload = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            ProfilePictureSource = ""
        };

        var registerResponse = await _client.PostAsJsonAsync("users/register", registrationPayload);
        registerResponse.EnsureSuccessStatusCode();

        if (!verify) return;

        await Task.Delay(TimeSpan.FromSeconds(2));

        var (token, parsedEmail) = ExtractTokenAndEmailFromEmail(email);
        Assert.NotNull(token);
        Assert.NotNull(parsedEmail);
        var verificationPayload = new { Email = parsedEmail, Token = token };
        var verifyResponse = await _client.PostAsJsonAsync("users/verify-email", verificationPayload);

        verifyResponse.EnsureSuccessStatusCode();
    }

    protected (string? Token, string? Email) ExtractTokenAndEmailFromEmail(string userEmail)
    {
        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(userEmail));
        if (sentEmail is null) return (null, null);

        var match = Regex.Match(
            sentEmail.Message.Body.Html.Data,
            @"href=['""](?<url>https?://[^'""]+/email-verification\?token=[^&]+&email=[^'""]+)['""]",
            RegexOptions.IgnoreCase);

        if (!match.Success) return (null, null);

        var url = System.Net.WebUtility.HtmlDecode(match.Groups["url"].Value);

        var uri = new Uri(url);

        var queryDict = QueryHelpers.ParseQuery(uri.Query);

        return (queryDict["token"].ToString(), queryDict["email"].ToString());
    }
}