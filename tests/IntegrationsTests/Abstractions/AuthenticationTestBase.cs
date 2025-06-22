using Application.Abstractions.BackgroundJobs;
using Application.Users.Login;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace IntegrationsTests.Abstractions;

public abstract class AuthenticationTestBase : BaseIntegrationTest
{
    protected string DefaultPassword => "TestPassword123!";
    protected string DefaultEmail => "test@gmail.com";
    protected AuthenticationTestBase(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    protected async Task TriggerOutboxProcess()
    {
        var outboxProcessor = _scope.ServiceProvider.GetRequiredService<IProcessOutboxMessagesJob>();
        await outboxProcessor.ExecuteAsync(null);
    }

    protected async Task<LoginResponse> LoginUser(string email, string password)
    {
        var loginPayload = new
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse.AccessToken);
        return loginResponse;
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

        await TriggerOutboxProcess();

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