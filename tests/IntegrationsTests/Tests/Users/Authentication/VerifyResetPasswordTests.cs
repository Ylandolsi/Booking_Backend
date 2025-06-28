using System.Net;
using System.Net.Http.Json;
using System.Web;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests.Users.Authentication;

public class VerifyResetPasswordTests : AuthenticationTestBase
{
    public VerifyResetPasswordTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task VerifyResetPassword_ShouldResetPassword_WhenTokenIsValid()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        var newPassword = "NewPassword123!";
        await RegisterAndVerifyUser(userEmail, DefaultPassword);

        EmailCapturer.Clear();  // delete the confirmation email from the registration process

        var resetRequestPayload = new { Email = userEmail };
        var resetResponse = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, resetRequestPayload);
        resetResponse.EnsureSuccessStatusCode();

        await Task.Delay(2000);  // wait for the email to be sent


        var (token, email) = ExtractTokenAndEmailFromEmail(userEmail);
        Assert.NotNull(token);
        Assert.NotNull(email);

        var decodedEmail = HttpUtility.UrlDecode(email);
        Assert.Equal(userEmail, decodedEmail);


        var verifyPayload = new
        {
            Email = userEmail,
            Token = token,
            Password = newPassword,
            ConfirmPassword = newPassword
        };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.ResetPassword, verifyPayload);

        verifyResponse.EnsureSuccessStatusCode();

        
        var loginResponse = await LoginUser(userEmail, newPassword);
        Assert.NotNull(loginResponse);
    }

    [Fact]
    public async Task VerifyResetPassword_ShouldResetPassword_WhenEmailIsNotValidated()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        var newPassword = "NewPassword123!";
        await RegisterAndVerifyUser(userEmail, DefaultPassword , false );
        EmailCapturer.Clear(); // delete the confirmation email from the registration process

        var resetRequestPayload = new { Email = userEmail };
        var resetResponse = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, resetRequestPayload);
        resetResponse.EnsureSuccessStatusCode();

        await Task.Delay(2000);  // wait for the email to be sent


        var (token, email) = ExtractTokenAndEmailFromEmail(userEmail);
        Assert.NotNull(token);
        Assert.NotNull(email);

        var decodedEmail = HttpUtility.UrlDecode(email);
        Assert.Equal(userEmail, decodedEmail);


        var verifyPayload = new
        {
            Email = userEmail,
            Token = token,
            Password = newPassword,
            ConfirmPassword = newPassword
        };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.ResetPassword, verifyPayload);

        verifyResponse.EnsureSuccessStatusCode();


        var loginResponse = await LoginUser(userEmail, newPassword);
        Assert.NotNull(loginResponse);
        
    }



    [Fact]
    public async Task VerifyResetPassword_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
    {
        EmailCapturer.Clear();
        var userEmail = Fake.Internet.Email();
        await RegisterAndVerifyUser(userEmail, DefaultPassword);

        EmailCapturer.Clear();



        var resetRequestPayload = new { Email = userEmail };
        var resetResponse = await _client.PostAsJsonAsync(UsersEndpoints.ForgotPassword, resetRequestPayload);
        resetResponse.EnsureSuccessStatusCode();


        await Task.Delay(2000);  // wait for the email to be sent

        var (token, email) = ExtractTokenAndEmailFromEmail(userEmail);
        Assert.NotNull(token);
        Assert.NotNull(email);  

        var decodedEmail = HttpUtility.UrlDecode(email);
        Assert.Equal(userEmail, decodedEmail);


        var verifyPayload = new
        {
            Email = userEmail,
            Token = token,
            Password = "NewPassword123!",
            ConfirmPassword = "DifferentPassword123!"
        };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.ResetPassword, verifyPayload);

        Assert.Equal(HttpStatusCode.BadRequest, verifyResponse.StatusCode);
    }

    [Fact]
    public async Task VerifyResetPassword_ShouldReturnBadRequest_WhenTokenIsInvalid()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var newPassword = "NewPassword123!";
        await RegisterAndVerifyUser(userEmail, DefaultPassword);

        var verifyPayload = new
        {
            Email = userEmail,
            Token = "invalid-token",
            Password = newPassword,
            ConfirmPassword = newPassword
        };
        var verifyResponse = await _client.PostAsJsonAsync(UsersEndpoints.ResetPassword, verifyPayload);

        Assert.Equal(HttpStatusCode.BadRequest, verifyResponse.StatusCode);
    }
}
