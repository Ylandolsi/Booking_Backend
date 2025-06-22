using Application.Abstractions.BackgroundJobs;
using Application.Users.Login;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel;
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

        var registerResponse = await _client.PostAsJsonAsync(UsersEndpoints.Register, registrationPayload);
        registerResponse.EnsureSuccessStatusCode();

        await TriggerOutboxProcess();

        if (!verify) return;

        await Task.Delay(TimeSpan.FromSeconds(2));

        var (token, parsedEmail) = ExtractTokenAndEmailFromEmail(email);
        Console.WriteLine($"Extracted token: {token}");
        Console.WriteLine($"Extracted email: {parsedEmail}");
        Assert.NotNull(token);
        Assert.NotNull(parsedEmail);

        var verificationPayload = new { Email = parsedEmail, Token = token };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.VerifyEmail, verificationPayload);

        verifyResponse.EnsureSuccessStatusCode();
    }


    protected (string? Token, string? Email) ExtractTokenAndEmailFromEmail(string userEmail)
    {
        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(userEmail));
        if (sentEmail is null) return (null, null);

        var match = Regex.Match(
            sentEmail.Message.Body.Html.Data,
            @"href=['""](?<url>https?://[^'""]+\?token=[^&]+&email=[^'""]+)['""]",
            RegexOptions.IgnoreCase);


        if (!match.Success) return (null, null);


        var url = match.Groups["url"].Value;

        var uri = new Uri(url);
        string query = uri.Query;
        // extract without decoding the URL (  decoding happens in the handler )
        string token = ExtractRawQueryParameter(query, "token");
        string email = ExtractRawQueryParameter(query, "email");

        return (token, email);
    }

    private string ExtractRawQueryParameter(string query, string paramName)
    {
        if (query.StartsWith("?")) query = query.Substring(1);

        var parameters = query.Split('&');

        foreach (var param in parameters)
        {
            var parts = param.Split('=');
            if (parts.Length == 2 && parts[0] == paramName)
            {
                return parts[1];
            }
        }

        return string.Empty;
    }
}