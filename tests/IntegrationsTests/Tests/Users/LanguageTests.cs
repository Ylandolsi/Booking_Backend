using System.Net;
using System.Net.Http.Json;
using Application.Users.Authentication.Utils;
using IntegrationsTests.Abstractions;

using LanguageE = Domain.Users.Entities.Language;


namespace IntegrationsTests.Tests.Users.Language;


public class LanguageTests : AuthenticationTestBase
{
    public LanguageTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllLanguages_ShouldReturnLanguages_ForAnyUser()
    {
        //UserData userData = await CreateUserAndLogin(); 

        var response = await _client.GetAsync(UsersEndpoints.GetAllLanguages);

        response.EnsureSuccessStatusCode();
        List<LanguageE> languages = await response.Content.ReadFromJsonAsync<List<LanguageE>>();

        Assert.NotNull(languages);
        Assert.NotEmpty(languages);
    }

    [Fact]
    public async Task GetUserLanguages_ShouldReturnUserLanguages_WhenUserIsAuthenticated()
    {
        UserData userData = await CreateUserAndLogin();

        var response = await _client.GetAsync(UsersEndpoints.GetUserLanguages.Replace("{userId:guid}", userData.UserId.ToString()));

        response.EnsureSuccessStatusCode();
        var languages = await response.Content.ReadFromJsonAsync<List<LanguageE>>();
        Assert.NotNull(languages);
    }
    [Fact]
    public async Task GetOtherUserLanguages_ShouldReturnUserLanguages()
    {
        UserData userData = await CreateUserAndLogin();
        UserData otherUserrData = await CreateUserAndLogin(); 

        var response = await _client.GetAsync(UsersEndpoints.GetUserLanguages.Replace("{userId:guid}", userData.UserId.ToString()));

        response.EnsureSuccessStatusCode();
        var languages = await response.Content.ReadFromJsonAsync<List<LanguageE>>();
        Assert.NotNull(languages);
    }

    [Fact]
    public async Task UpdateUserLanguages_ShouldUpdateLanguages_WhenUserIsAuthenticated()
    {
        UserData userData = await CreateUserAndLogin();

        var languagesResponse = await _client.GetAsync(UsersEndpoints.GetAllLanguages);
        languagesResponse.EnsureSuccessStatusCode();

        var allLanguages = await languagesResponse.Content.ReadFromJsonAsync<List<LanguageE>>();

        Assert.NotNull(allLanguages);
        Assert.NotEmpty(allLanguages);



        var updatePayload = new { LanguageIds = new List<int> { allLanguages[0].Id , allLanguages[1].Id } };
        var response = await _client.PutAsJsonAsync(UsersEndpoints.UpdateUserLanguages, updatePayload);

        response.EnsureSuccessStatusCode();

        // Verify the user now has these languages
        var userLanguagesResponse = await _client.GetAsync(UsersEndpoints.GetUserLanguages.Replace("{userId:guid}", userData.UserId.ToString()));

        userLanguagesResponse.EnsureSuccessStatusCode();
        var userLanguages = await userLanguagesResponse.Content.ReadFromJsonAsync<List<Domain.Users.Entities.Language>>();
        Assert.NotNull(userLanguages);
        Assert.Equal(2, userLanguages.Count);
    }

}