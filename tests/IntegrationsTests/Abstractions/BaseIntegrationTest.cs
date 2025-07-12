using Amazon.SimpleEmail.Model;
using Azure;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationsTests.Abstractions;

[Collection(nameof(IntegrationTestsCollection))]
public abstract class BaseIntegrationTest : IDisposable, IAsyncLifetime
{
    protected readonly Faker Fake = new();
    protected readonly IServiceScope _scope;
    protected readonly IntegrationTestsWebAppFactory Factory;
    protected List<SendEmailRequest> EmailCapturer => Factory.CapturedEmails;

    // authentications purpose
    // users claims wihtout really creating a user ( not persisted in the database )
    // for endpoints that require authentication and authorization
    protected readonly Guid _verifiedUserId = Guid.NewGuid();
    protected readonly string _verifiedUserEmail = "verified.user@example.com";
    protected readonly HttpClient _client;
    protected readonly HttpClient _authenticatedVerifiedUserClient;
    private readonly Func<Task> _resetDatabase;


    public BaseIntegrationTest(IntegrationTestsWebAppFactory factory)
    {
        Factory = factory;
        _scope = Factory.Services.CreateScope();
        _resetDatabase = factory.ResetDatabase;
        EmailCapturer?.Clear();


        _client = Factory.CreateClient();
        //_authenticatedVerifiedUserClient = Factory.CreateAuthenticatedClient(
        //    userId: _verifiedUserId,
        //    email: _verifiedUserEmail,
        //    isEmailVerified: true);
    }

    protected bool IsSucceed(int statusCode) // Change parameter type to int
    {
        return statusCode == StatusCodes.Status200OK ||
               statusCode == StatusCodes.Status201Created ||
               statusCode == StatusCodes.Status204NoContent;
    }

    protected void CheckSuccess(HttpResponseMessage response) => Assert.True(IsSucceed((int)response.StatusCode), "The response status code does not indicate success.");


    public async Task InitializeAsync()
    {
        await _resetDatabase();
        EmailCapturer?.Clear();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}