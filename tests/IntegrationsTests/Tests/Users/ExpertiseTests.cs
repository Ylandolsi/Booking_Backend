using System.Net.Http.Json;
using IntegrationsTests.Abstractions;
using ExpertiseE = Domain.Users.Entities.Expertise;
namespace IntegrationsTests.Tests.Users;

public class ExpertiseTests : AuthenticationTestBase
{
    public ExpertiseTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllExpertises_ShouldReturnExpertises_WhenUserIsUnAuthenticated()
    {

        var response = await _client.GetAsync(UsersEndpoints.GetAllExpertises);

        response.EnsureSuccessStatusCode();
        var expertises = await response.Content.ReadFromJsonAsync<List<ExpertiseE>>();
        Assert.NotNull(expertises);
        Assert.NotEmpty(expertises);
    }

    [Fact]
    public async Task GetUserExpertises_ShouldReturnUserExpertises_WhenUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin();

        var response = await _client.GetAsync(UsersEndpoints.GetUserExpertises.Replace("{userSlug}", userData.UserSlug.ToString()));

        response.EnsureSuccessStatusCode();
        var expertises = await response.Content.ReadFromJsonAsync<List<ExpertiseE>>();
        Assert.NotNull(expertises);
    }
    [Fact]
    public async Task GetOtherUserExpertises_ShouldReturnUserExpertises_WhenUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin();
        var useData2 = await CreateUserAndLogin();

        var response = await _client.GetAsync(UsersEndpoints.GetUserExpertises.Replace("{userSlug}", userData.UserSlug.ToString()));

        response.EnsureSuccessStatusCode();
        var expertises = await response.Content.ReadFromJsonAsync<List<ExpertiseE>>();
        Assert.NotNull(expertises);
    }

    [Fact]
    public async Task UpdateUserExpertise_ShouldUpdateExpertises_WhenUserIsAuthenticated()
    {
        var userData = await CreateUserAndLogin();


        // Get all available expertises
        var expertisesResponse = await _client.GetAsync(UsersEndpoints.GetAllExpertises);
        expertisesResponse.EnsureSuccessStatusCode();

        List<ExpertiseE>? allExpertises = await expertisesResponse.Content.ReadFromJsonAsync<List<ExpertiseE>>();

        Assert.NotNull(allExpertises);
        Assert.NotEmpty(allExpertises);

        var updatePayload = new { ExpertiseIds = new List<int> { allExpertises[0].Id, allExpertises[1].Id } };

        var response = await _client.PutAsJsonAsync(UsersEndpoints.UpdateUserExpertise, updatePayload);

        response.EnsureSuccessStatusCode();

        var userExpertisesResponse = await _client.GetAsync(UsersEndpoints.GetUserExpertises.Replace("{userSlug}", userData.UserSlug.ToString()));

        userExpertisesResponse.EnsureSuccessStatusCode();
        var userExpertises = await userExpertisesResponse.Content.ReadFromJsonAsync<List<ExpertiseE>>();

        Assert.NotNull(userExpertises);
        Assert.Equal(2, userExpertises.Count);
    }


}