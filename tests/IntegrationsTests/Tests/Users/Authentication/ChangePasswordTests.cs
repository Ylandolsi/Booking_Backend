using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Users.Authentication.ChangePassword;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests;

public class ChangePasswordTests : AuthenticationTestBase
{
    public ChangePasswordTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    //     var (userId, httpClient) = await GetAuthenticatedUserAndClientAsync();
    [Fact]
    public async Task ChangePassword_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange

        var request = new
        {
            OldPassword = "oldPassword123!",
            NewPassword = "newPassword123!",
            ConfirmNewPassword = "newPassword123!"
        };

        // Act
        var response = await _client.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }

    [Fact]
    public async Task ChangePassword_Should_ReturnBadRequest_WhenNewPasswordsDoNotMatch()
    {
        // Arrange
        var userData = await CreateUserAndLogin();

        var request = new
        {
            OldPassword = DefaultPassword,
            NewPassword = "newPassword123!",
            ConfirmNewPassword = "passwordsDoNotMatch456!"
        };
        // cookies are sent automatically

        var response = await _client.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }

    [Fact]
    public async Task ChangePassword_Should_ReturnBadRequest_WhenOldPasswordIsIncorrect()
    {

        await RegisterAndVerifyUser(DefaultEmail, DefaultPassword, true);
        await LoginUser(DefaultEmail, DefaultPassword);

        var client = Factory.CreateClient();

        var loginPayload = new { Email = DefaultEmail, Password = DefaultPassword };
        var loginResult = await client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);
        loginResult.EnsureSuccessStatusCode();

        var request = new
        {
            OldPassword = "incorrectOldPasswords",
            NewPassword = "newPassword123!",
            ConfirmNewPassword = "passwordsDoNotMatch456!"
        };

        var response = await client.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_Should_ReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await RegisterAndVerifyUser(DefaultEmail, DefaultPassword, true);
        await LoginUser(DefaultEmail, DefaultPassword);

        var client = Factory.CreateClient();

        var loginPayload = new { Email = DefaultEmail, Password = DefaultPassword };
        var loginResult = await client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);
        loginResult.EnsureSuccessStatusCode();

        var request = new
        {
            OldPassword = DefaultPassword,
            NewPassword = "newPassword123!",
            ConfirmNewPassword = "newPassword123!"
        };

        var response = await client.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

        CheckSuccess(response);
    }
}