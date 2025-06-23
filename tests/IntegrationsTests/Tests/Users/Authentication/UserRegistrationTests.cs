using Application.Abstractions.BackgroundJobs;
using IntegrationsTests.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationsTests.Tests;

public class UserRegistrationTests : AuthenticationTestBase
{
    public UserRegistrationTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterUser_ShouldSendVerificationEmail()
    {
        // Arrange
        EmailCapturer.Clear();
        var registrationPayload = new
        {
            FirstName = "Test",
            LastName = "UserReg",
            Email = Fake.Internet.Email(),
            Password = "Password123!",
            ProfilePictureSource = ""
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(UsersEndpoints.Register, registrationPayload);

        // Assert
        response.EnsureSuccessStatusCode();


        await TriggerOutboxProcess();

        await Task.Delay(TimeSpan.FromSeconds(2));
        var sentEmail = EmailCapturer.FirstOrDefault(e => e.Destination.ToAddresses.Contains(registrationPayload.Email));
        Assert.NotNull(sentEmail);
        Assert.NotNull(sentEmail.Message.Body.Html.Data);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailIsAlreadyInUse()
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
        await _client.PostAsJsonAsync(UsersEndpoints.Register, payload);

        // Act
        var secondResponse = await _client.PostAsJsonAsync(UsersEndpoints.Register, payload);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, secondResponse.StatusCode);
    }
    [Theory]
    [InlineData("invalid-email", "Password123!")]
    [InlineData("test@test.com", "short")]
    public async Task Register_ShouldReturnBadRequest_WhenInputIsInvalid(string email, string password)
    {
        var payload = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            ProfilePictureSource = ""
        };

        var response = await _client.PostAsJsonAsync(UsersEndpoints.Register, payload);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}