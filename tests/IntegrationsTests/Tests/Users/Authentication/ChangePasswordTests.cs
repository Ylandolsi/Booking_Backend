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
        await RegisterAndVerifyUser(DefaultEmail, DefaultPassword, true);
        var loginResponse = await LoginUser(DefaultEmail, DefaultPassword);

        // Use the real access token from the login response for authentication
        var authenticatedClient = Factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        var request = new
        {
            OldPassword = DefaultPassword, // Use the correct old password
            NewPassword = "newPassword123!",
            ConfirmNewPassword = "passwordsDoNotMatch456!"
        };

        // Act
        var response = await authenticatedClient.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

        authenticatedClient.DefaultRequestHeaders.Authorization = null;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }

    // [Fact]
    // public async Task ChangePassword_Should_ReturnBadRequest_WhenOldPasswordIsIncorrect()
    // {
    //     // Arrange
    //     var (userId, httpClient) = await GetAuthenticatedUserAndClientAsync();
    //     var request = new ChangePassword.Request(
    //         "incorrectOldPassword",
    //         "newPassword123!",
    //         "newPassword123!");

    //     // Act
    //     var response = await httpClient.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

    //     // Assert
    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    //     var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
    //     problemDetails.ErrorCode.Should().Contain("ChangePassword.Failed");
    // }

    // [Fact]
    // public async Task ChangePassword_Should_ReturnOk_WhenRequestIsValid()
    // {
    //     // Arrange
    //     var (userId, httpClient) = await GetAuthenticatedUserAndClientAsync();
    //     var newPassword = "newValidPassword123!";
    //     var request = new ChangePassword.Request(
    //         BaseIntegrationTest.DefaultPassword,
    //         newPassword,
    //         newPassword);

    //     // Act
    //     var response = await httpClient.PutAsJsonAsync(UsersEndpoints.ChangePassword, request);

    //     // Assert
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }
}