using Application.Abstractions.BackgroundJobs;
using Application.Users.Authentication.Utils;
using Bogus;
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

        var registerResponse = await _client.PostAsJsonAsync(UsersEndpoints.Register, registrationPayload);
        registerResponse.EnsureSuccessStatusCode();


        await TriggerOutboxProcess();

        await Task.Delay(TimeSpan.FromSeconds(2));
        if (!verify) return;


        var (token, parsedEmail) = ExtractTokenAndEmailFromEmail(email);

        Assert.NotNull(token);
        Assert.NotNull(parsedEmail);

        var verificationPayload = new { Email = parsedEmail, Token = token };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.VerifyEmail, verificationPayload);

        //verifyResponse.EnsureSuccessStatusCode();
        CheckSuccess(verifyResponse);
    }
    protected async Task<LoginResponse> CreateUserAndLogin()
    {
        string? email = Fake.Internet.Email();
        await RegisterAndVerifyUser(email, DefaultPassword, true);
        return await LoginUser(email, DefaultPassword);

    }

    protected (string? Token, string? Email) ExtractTokenAndEmailFromEmail(string userEmail)
    {

        var sentEmail = EmailCapturer.LastOrDefault(e => e.Destination.ToAddresses.Contains(userEmail));
        if (sentEmail is null) return (null, null);

        var match = Regex.Match(
            sentEmail.Message.Body.Html.Data,
            @"href=['""](?<url>https?://[^'""]+\?token=[^&]+&email=[^'""]+)['""]",
            RegexOptions.IgnoreCase);

        if (!match.Success) return (null, null);

        var url = match.Groups["url"].Value;
        var uri = new Uri(url);

        // Use QueryHelpers to parse and decode the query string
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        queryParams.TryGetValue("token", out var token);
        queryParams.TryGetValue("email", out var email);

        return (token.ToString(), email.ToString());
    }

}