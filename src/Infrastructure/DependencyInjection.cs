using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.BackgroundJobs.TokenCleanup;
using Application.Abstractions.Data;
using Application.Options;
using Domain.Users.Entities;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.BackgroundJobs.SendingVerificationEmail;
using Infrastructure.BackgroundJobs.TokenCleanup;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using SharedKernel;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddOptions(configuration)
            .AddSESAWS(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddBackgroundJobs();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }


    public static IServiceCollection AddIdentityCore(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(
            o =>
            {
                o.User.RequireUniqueEmail = true;
                o.SignIn.RequireConfirmedAccount = true;
                o.Password.RequiredLength = 8;
                o.Password.RequireDigit = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;

                // Configure lockout settings
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.AllowedForNewUsers = true;
            }
        )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        return services;
    }
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddIdentityCore();

        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Consider logging this or throwing a more specific exception
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services
            .AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JwtOptionsKey));
        services.Configure<GoogleOAuthOptions>(configuration.GetSection(GoogleOAuthOptions.GoogleOptionsKey));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.EmailOptionsKey));
        services.Configure<FrontendApplicationOptions>(configuration.GetSection(FrontendApplicationOptions.FrontEndOptionsKey));
        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
            )
            .AddJwtBearer(o =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.JwtOptionsKey)
                                              .Get<JwtOptions>() ??
                                              throw new InvalidOperationException("JWT options are not configured.");


                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => // how to retrieve the token from the request 
                    {
                        context.Token = context.Request.Cookies["access_token"] ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(options =>
            {
                var googleOptions = configuration.GetSection(GoogleOAuthOptions.GoogleOptionsKey)
                                                 .Get<GoogleOAuthOptions>() ?? throw new InvalidOperationException("Google Oauth is not configured");

                options.ClientId = googleOptions.ClientId!;
                options.ClientSecret = googleOptions.ClientSecret!;
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenWriterCookies, TokenWriterCookies>();
        services.AddSingleton<IEmailVerificationLinkFactory, EmailVerificationLinkFactory>();

        return services;
    }

    private static IServiceCollection AddSESAWS(this IServiceCollection services,
                                                IConfiguration configuration)
    {
        // TODO : use environment variables or secrets manager for sensitive data
        var awsOptions = configuration.GetSection("AWS");
        var awsAccessKey = awsOptions["AccessKey"];
        var awsSecretKey = awsOptions["SecretKey"];
        var awsRegion = awsOptions["Region"];

        services.AddDefaultAWSOptions(new AWSOptions
        {
            Credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey),
            Region = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
        });
        services.AddHttpClient<IAmazonSimpleEmailService, AmazonSimpleEmailServiceClient>()
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true; // smooth the retry attempts


                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
            });

        return services;
    }


    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddScoped<IRegisterVerificationJob, VerificationEmailForRegistrationJob>();
        services.AddScoped<ITokenCleanupJob, TokenCleanupJob>();

        return services;

    }


}
