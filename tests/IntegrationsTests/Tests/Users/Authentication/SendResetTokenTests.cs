using System.Net;
using System.Net.Http.Json;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests.Users.Authentication;

public class SendResetTokenTests : AuthenticationTestBase
{
    public SendResetTokenTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SendResetToken_ShouldSendEmail_WhenEmailExists()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, DefaultPassword);

        EmailCapturer.Clear(); // delete regisration email ! 
        var requestPayload = new { Email = userEmail };


        var response = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, requestPayload);

        // Assert
        response.EnsureSuccessStatusCode();

        await Task.Delay(2000); // Wait for the email to be sent

        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(userEmail));
        Assert.NotNull(sentEmail);
        Assert.Contains("Password Reset Request", sentEmail.Message.Body.Html.Data);
    }
    [Fact]
    public async Task SendResetToken_ShouldSendEmail_WhenEmailIsNotVerified()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, DefaultPassword , false);

        var requestPayload = new { Email = userEmail };


        EmailCapturer.Clear(); // delete regisration email ! 
        var response = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, requestPayload);

        // Assert
        response.EnsureSuccessStatusCode();

        await Task.Delay(2000); // Wait for the email to be sent

        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(userEmail));
        Assert.NotNull(sentEmail);
        Assert.Contains("Password Reset Request", sentEmail.Message.Body.Html.Data);
    }

    [Fact]
    public async Task SendResetToken_ShouldReturnOk_WhenEmailDoesNotExist()
    {
        // Arrange
        EmailCapturer.Clear();
        var nonExistentEmail = Fake.Internet.Email();

        var requestPayload = new { Email = nonExistentEmail };

        // Act
        var response = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, requestPayload);

        await Task.Delay(2000); // Wait for the email to be sent

        // Assert
        response.EnsureSuccessStatusCode();

        // Ensure no email was sent
        Assert.Empty(EmailCapturer);
    }
}