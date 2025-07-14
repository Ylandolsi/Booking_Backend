using System.Net;
using System.Net.Http.Json;
using Application.Users.Authentication.Utils;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests;

public class UserLoginTests : AuthenticationTestBase
{
    public UserLoginTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword);

        // Act
        var loginPayload = new { Email = userEmail, Password = userPassword };
        var loginResponse = await _client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);

        // Assert
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResult);
       
    }
    [Fact]
    public async Task GetCurrentUser_ShouldReturnValidAnswer_WhenTheUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin();

        var currentUserResponse = await _client.GetAsync(UsersEndpoints.GetCurrentUser);
        Assert.NotNull(currentUserResponse);
        Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);
        Assert.Equal(userData.Email, userData.Email);
    }

        [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword);

        // Act
        var loginPayload = new { Email = userEmail, Password = "WrongPassword!" };
        var loginResponse = await _client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, loginResponse.StatusCode);
    }
    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenEmailIsNotVerified()
    {
        // Arrange
        var userEmail = Fake.Internet.Email();
        var userPassword = "Password123!";
        await RegisterAndVerifyUser(userEmail, userPassword, verify: false);

        // Act
        var loginPayload = new { Email = userEmail, Password = userPassword };
        var loginResponse = await _client.PostAsJsonAsync(UsersEndpoints.Login, loginPayload);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

    }
}