
using Application.Users.Login;
using IntegrationsTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;


namespace IntegrationsTests;


public class AuthenticationTest : BaseIntegrationTest
{
    protected readonly Guid _verifiedUserId;
    protected readonly string _verifiedUserEmail;
    protected readonly HttpClient _client;
    protected readonly HttpClient _authenticatedVerifiedUserClient;

    public AuthenticationTest(IntegrationTestsWebAppFactory factory) : base(factory)
    {

        _client = Factory.CreateClient();
        _verifiedUserId = base._verifiedUserId;
        _verifiedUserEmail = base._verifiedUserEmail;
        _authenticatedVerifiedUserClient = base._authenticatedVerifiedUserClient;
    }
    [Fact]
    public async Task RegisterUser_ShouldSendVerificationEmail_AndReturnUserId()
    {
        EmailCapturer.Clear();

        var registrationPayload = new
        {
            FirstName = "Test",
            LastName = "UserReg",
            Email = Fake.Internet.Email(),
            Password = "Password123!",
            ProfilePictureSource = ""
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("users/register", registrationPayload);
        response.EnsureSuccessStatusCode();

        await Task.Delay(TimeSpan.FromSeconds(2));

        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(registrationPayload.Email));

        Assert.NotNull(sentEmail);
        Assert.Contains("click here", sentEmail.Message.Body.Html.Data, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReSendVerificationEmail_ShouldSendNewVerificationEmail_WhenUserNotVerified()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword);

        var resendPayload = new { Email = Fake.Internet.Email() };
        HttpResponseMessage response = await _authenticatedVerifiedUserClient.PostAsJsonAsync("users/resend-verification-email", resendPayload);

        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReSendVerificationEmail_ShouldReturnError_WhenUserAlreadyVerified()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword , false );


        var resendPayload = new { Email = _verifiedUserEmail };
        HttpResponseMessage response = await _authenticatedVerifiedUserClient.PostAsJsonAsync("users/resend-verification-email", resendPayload);

        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValidAndEmailIsVerified()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword);

        // Act
        var loginPayload = new { Email = userEmail, Password = userPassword };
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginPayload);

        // Assert
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();

        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.AccessToken));
        Assert.Equal(userEmail, loginResult.Email);
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenPasswordIsIncorrect()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword);

        // Act
        var loginPayload = new { Email = userEmail, Password = "WrongPassword!" };
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginPayload);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task VerifyEmail_ShouldFail_WhenTokenIsInvalid()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword, verify: false);

        // Act
        var verificationPayload = new { Email = userEmail, Token = "invalid-token" };
        var verifyResponse = await _client.PostAsJsonAsync("users/verify-email", verificationPayload);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, verifyResponse.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenEmailIsAlreadyInUse()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var payload = new
        {
            FirstName = "Duplicate",
            LastName = "User",
            Email = userEmail,
            Password = "Password123!",
            ProfilePictureSource = ""
        };

        // 1. Register the first user
        var firstResponse = await _client.PostAsJsonAsync("users/register", payload);
        firstResponse.EnsureSuccessStatusCode();

        // Act: Attempt to register the second user with the same email
        var secondResponse = await _client.PostAsJsonAsync("users/register", payload);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, secondResponse.StatusCode);
    }

    #region Helper Methods

    private async Task RegisterAndVerifyUser(string email, string password, bool verify = true)
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



        var ( token , parsedEmail ) = ExtractTokenAndEmailFromEmail(email);
        Assert.NotNull(token);
        Assert.NotNull(parsedEmail);
        var verificationPayload = new { Email = parsedEmail, Token = token };
        var verifyResponse = await _client.PostAsJsonAsync("users/verify-email", verificationPayload);

        verifyResponse.EnsureSuccessStatusCode();
    }

    private (string? Token, string? Email) ExtractTokenAndEmailFromEmail(string userEmail)
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




    #endregion
}


