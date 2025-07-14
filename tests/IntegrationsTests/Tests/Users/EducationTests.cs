using System.Net;
using System.Net.Http.Json;
using Application.Users.Authentication.Utils;
using IntegrationsTests.Abstractions;

namespace IntegrationsTests.Tests.Users.Education;

public class EducationTests : AuthenticationTestBase
{
    public EducationTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AddEducation_ShouldCreateEducation_WhenUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin(); 

        var educationPayload = new
        {
            Field = "Computer Science",
            University = "Test University",
            StartDate = DateTime.UtcNow.AddYears(-4),
            EndDate = DateTime.UtcNow.AddYears(-1),
            Description = "Bachelor's degree in Computer Science"
        };

        var response = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);

        response.EnsureSuccessStatusCode();

    }

    [Fact]
    public async Task GetEducations_ShouldReturnEducations_WhenUserIsAuthenticated()
    {
        var userData = await  CreateUserAndLogin();

        // Add an education first
        var educationPayload = new
        {
            Field = "Computer Science",
            University = "Test University",
            StartDate = DateTime.UtcNow.AddYears(-4),
            EndDate = DateTime.UtcNow.AddYears(-1),
            Description = "Bachelor's degree in Computer Science"
        };
        await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);

        // Act
        var response = await _client.GetAsync(UsersEndpoints.GetUserEducations.Replace("{userSlug}", userData.UserSlug));

        // Assert
        response.EnsureSuccessStatusCode();
        var educations = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(educations);
        Assert.NotEmpty(educations);
    }

    [Fact]
    public async Task UpdateEducation_ShouldUpdateEducation_WhenUserIsAuthenticated()
    {
        LoginResponse loginResponse = await CreateUserAndLogin();

        // Add an education first
        var educationPayload = new
        {
            Field = "Computer Science",
            University = "Test University",
            StartDate = DateTime.UtcNow.AddYears(-4),
            EndDate = DateTime.UtcNow.AddYears(-1),
            Description = "Bachelor's degree in Computer Science"
        };
        var createResponse = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);
        createResponse.EnsureSuccessStatusCode();
        
        var educationId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Update the education
        var updatePayload = new
        {
            Field = "Software Engineering",
            University = "Updated University",
            StartDate = DateTime.UtcNow.AddYears(-5),
            EndDate = DateTime.UtcNow.AddYears(-2),
            Description = "Master's degree in Software Engineering"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            UsersEndpoints.UpdateEducation.Replace("{educationId}", educationId.ToString()), 
            updatePayload);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteEducation_ShouldRemoveEducation_WhenUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin();

        // Add an education first
        var educationPayload = new
        {
            Field = "Computer Science",
            University = "Test University",
            StartDate = DateTime.UtcNow.AddYears(-4),
            EndDate = DateTime.UtcNow.AddYears(-1),
            Description = "Bachelor's degree in Computer Science"
        };
        var createResponse = await _client.PostAsJsonAsync(UsersEndpoints.AddEducation, educationPayload);
        createResponse.EnsureSuccessStatusCode();

        var educationId = await createResponse.Content.ReadFromJsonAsync<int>();    

        // Act
        var response = await _client.DeleteAsync(
            UsersEndpoints.DeleteEducation.Replace("{educationId}", educationId.ToString()));

        // Assert
        response.EnsureSuccessStatusCode();

        // Verify it's deleted
        var getResponse = await _client.GetAsync(UsersEndpoints.GetUserEducations.Replace("{userSlug}", userData.UserSlug));
        getResponse.EnsureSuccessStatusCode();
        var educations = await getResponse.Content.ReadFromJsonAsync<List<object>>();
        Assert.Empty(educations);
    }
}