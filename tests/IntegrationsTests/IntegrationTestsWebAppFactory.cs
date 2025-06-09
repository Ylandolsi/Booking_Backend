using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Web.Api;

namespace IntegrationsTests;
// instaniate in memory aspnetcore web api 
public class IntegrationTestsWebAppFactory : WebApplicationFactory<Program>,  IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("MentorMentee")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
     
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync(); 
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Here you can configure your services for testing, like replacing the database context with an in-memory one
            // or mocking dependencies.
        });
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}