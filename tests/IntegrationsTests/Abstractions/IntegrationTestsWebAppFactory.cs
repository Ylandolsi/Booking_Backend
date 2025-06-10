using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Web.Api;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using FluentEmail.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace IntegrationsTests.Abstractions;

public class IntegrationTestsWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("MentorMentee__test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Corrected the environment variable key to "ConnectionStrings:Database"
        Environment.SetEnvironmentVariable("ConnectionStrings:Database",
            _dbContainer.GetConnectionString());

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
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });
               
            // IFluentEmail itself dosnet send the emails : 
            // it delegate that to FluentEmail.Core.Interfaces.ISender
            // ==> we are mocking that ISender 

            services.RemoveAll<ISender>();
            services.AddSingleton<CapturingSender>();
            services.AddSingleton<ISender>(sp => sp.GetRequiredService<CapturingSender>());

            services.AddFluentEmail("testdefault@example.com");
        });
    }

    public HttpClient CreateAuthenticatedClient(Guid? userId = null, string? email = null, bool isEmailVerified = true, IEnumerable<Claim>? additionalClaims = null)
    {
        var client = CreateClient();
        var testUserId = userId ?? Guid.NewGuid();
        var testEmail = email ?? "testuser@example.com";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, testUserId.ToString()),
            new(JwtRegisteredClaimNames.Email, testEmail),
            new("IsEmailVerified", isEmailVerified.ToString().ToLowerInvariant())
        };

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        client.DefaultRequestHeaders.Add(TestAuthHandler.TestUserClaimsHeader, JsonSerializer.Serialize(claims.Select(c => new { c.Type, c.Value })));
        return client;
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}