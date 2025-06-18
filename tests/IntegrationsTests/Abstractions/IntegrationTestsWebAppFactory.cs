using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentEmail.Core.Interfaces;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Testcontainers.PostgreSql;
using Web.Api;

namespace IntegrationsTests.Abstractions;

// docker pull datalust/seq:latest
// docker ps 
    //private readonly IContainer _seqContainer = new ContainerBuilder()
    //            .WithImage("datalust/seq:latest")
    //            .WithName("seq-test-container")
    //            .WithEnvironment("ACCEPT_EULA", "Y")
    //            .WithPortBinding(80, true)
    //            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
    //            .Build();
public class IntegrationTestsWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;


    private Respawner _respawner = default!;
    private string _connectionString = default!;
    private DbConnection _dbConnection = default!;

    public List<SendEmailRequest> CapturedEmails { get; private set; } = new();

    public IntegrationTestsWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("MentorMentee__test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();



        // Start the container and set the connection string synchronously
        _dbContainer.StartAsync().GetAwaiter().GetResult();
        _connectionString = _dbContainer.GetConnectionString();

        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _connectionString);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });


            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAmazonSimpleEmailService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockSes = CaptureAmazonSESServiceMock.CreateMock(out List<SendEmailRequest> capturedEmails);
            CapturedEmails = capturedEmails;
            services.AddSingleton<IAmazonSimpleEmailService>(mockSes);




        });


    }


    public HttpClient CreateAuthenticatedClient(Guid? userId = null, string? email = null, bool isEmailVerified = true)
    {
        var client = CreateClient();

        var claims = new List<object>
        {
            new { Type = JwtRegisteredClaimNames.Sub, Value = (userId ?? Guid.NewGuid()).ToString() },
        };

        client.DefaultRequestHeaders.Add(
            TestAuthHandler.TestUserClaimsHeader,
            JsonSerializer.Serialize(claims));

        return client;
    }



    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        await InitializeDbRespawner();
    }


    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();

        if (_dbConnection?.State == System.Data.ConnectionState.Open)
        {
            await _dbConnection.CloseAsync();
        }
        _dbConnection?.Dispose();

        NpgsqlConnection.ClearAllPools();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabase()
    {

        await _respawner.ResetAsync(_dbConnection);
    }


    private async Task InitializeDbRespawner()
    {
        _connectionString = _dbContainer.GetConnectionString();
        _dbConnection = new NpgsqlConnection(_connectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }


}