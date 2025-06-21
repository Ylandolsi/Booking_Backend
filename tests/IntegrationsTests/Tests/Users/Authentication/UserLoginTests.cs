using System.Net;
using System.Net.Http.Json;
using Application.Users.Login;
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
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginPayload);

        // Assert
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.AccessToken));
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
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginPayload);

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
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginPayload);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

    }
}