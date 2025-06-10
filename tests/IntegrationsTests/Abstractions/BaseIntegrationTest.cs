using Bogus;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationsTests.Abstractions;

[Collection(nameof(IntegrationTestsCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    protected readonly Faker Fake = new();
    protected readonly IServiceScope _scope;
    protected readonly IntegrationTestsWebAppFactory Factory; 

    protected CapturingSender EmailCapturer => _scope.ServiceProvider.GetRequiredService<CapturingSender>();

    public BaseIntegrationTest(IntegrationTestsWebAppFactory factory)
    {
        Factory = factory; 
        _scope = Factory.Services.CreateScope();
        EmailCapturer?.Clear();
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this); 
    }
}