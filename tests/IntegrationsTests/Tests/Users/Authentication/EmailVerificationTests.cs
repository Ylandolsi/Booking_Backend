using System.Net;
using System.Net.Http.Json;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests;

public class EmailVerificationTests : AuthenticationTestBase
{
    public EmailVerificationTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ReSendVerificationEmail_ShouldSendNewEmail_WhenUserNotVerified()
    {

        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, "Password123!", verify: false  );

        // Act
        var resendPayload = new { Email = userEmail };
        var response = await _client.PostAsJsonAsync(UsersEndpoints.ResendVerificationEmail, resendPayload);

        // Assert
        response.EnsureSuccessStatusCode();
        await Task.Delay(TimeSpan.FromSeconds(2));
        Assert.Equal(2, EmailCapturer.Where(e => e.Destination.ToAddresses.Contains(userEmail)).Count());
    }

    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenTokenIsInvalid()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, "Password123!", verify: false);

        // Act
        var verificationPayload = new { Email = userEmail, Token = "invalid-token" };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.VerifyEmail, verificationPayload);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, verifyResponse.StatusCode);
    }


    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenLinkIsUsedTwice()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, "Password123!", verify: false );
        await Task.Delay(TimeSpan.FromSeconds(2)); // Allow email to be "sent"
        var (token, parsedEmail) = ExtractTokenAndEmailFromEmail(userEmail);
        Assert.NotNull(token);
        Assert.NotNull(parsedEmail);
        var verificationPayload = new { Email = parsedEmail, Token = token };

        // Act
        // First attempt
        var firstResponse = await _client.PostAsJsonAsync(UsersEndpoints.VerifyEmail, verificationPayload);

        // Second attempt
        var secondResponse = await _client.PostAsJsonAsync(UsersEndpoints.VerifyEmail, verificationPayload);

        // Assert
        firstResponse.EnsureSuccessStatusCode();
        Assert.NotEqual(HttpStatusCode.OK, secondResponse.StatusCode);
    }
}