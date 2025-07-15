using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleEmail;
using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs;
using Application.Abstractions.BackgroundJobs.SendingPasswordResetToken;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.BackgroundJobs.TokenCleanup;
using Application.Abstractions.Data;
using Application.Abstractions.Email;
using Application.Abstractions.Uploads;
using Application.Options;
using Domain.Users.Entities;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.BackgroundJobs;
using Infrastructure.BackgroundJobs.Cleanup;
using Infrastructure.BackgroundJobs.SendingPasswordResetToken;
using Infrastructure.BackgroundJobs.SendingVerificationEmail;
using Infrastructure.BackgroundJobs.TokenCleanup;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Email;
using Infrastructure.Time;
using Infrastructure.Uploads;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using SharedKernel;
using System.Security.Cryptography;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddCors()
            .AddServices()
            .AddCache(configuration)
            .AddResielenecPipelines(configuration)
            .AddOptions(configuration)
            .AddAWS(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddBackgroundJobs();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<IEmailService, AwsSesEmailService>();
        services.AddSingleton<IEmailTemplateProvider, EmailTemplateProvider>();
        // AWS UPLOAD
        // services.AddScoped<IS3ImageProcessingService , S3ImageProcessingService>();
        services.AddScoped<IS3ImageProcessingService, LocalFileImageProcessingService>();

        services.AddScoped<ISlugGenerator, SlugGenerator.SlugGenerator>();

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", builder =>
            {
                builder.WithOrigins("http://localhost:3000",
                                    "http://localhost:3000",
                                    "http://localhost:5000")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                // TODO 
                // For production 
                // .WithOrigins("https://yourproductiondomain.com")

                // If you need to expose custom headers
                // .WithExposedHeaders("access_token", "refresh_token");
            });
        });
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("Cache") ?? throw new InvalidOperationException("Redis connection string is not configured.");
        });
        return services;
    }


    public static IServiceCollection AddIdentityCore(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<int>>(
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

                // configure token providers
                o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
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
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default);

                })
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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
                                              .Get<JwtOptions>()?.AccessToken ??
                                              throw new InvalidOperationException("JWT options are not configured.");


                var rsa = RSA.Create();


                var publicKey = jwtOptions.PublicKey
                                .Replace("\\n", "\n")
                                .Trim();


                rsa.ImportFromPem(publicKey.ToCharArray());
                Console.WriteLine("Successfully imported using ImportRSAPublicKey");




                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.FromSeconds(30)

                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => // how to retrieve the token from the request 
                    {
                        context.Token = context.Request.Cookies["access_token"] ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };

            }
            )

            .AddGoogle(options =>
            {
                var googleOptions = configuration.GetSection(GoogleOAuthOptions.GoogleOptionsKey)
                                                 .Get<GoogleOAuthOptions>() ?? throw new InvalidOperationException("Google Oauth is not configured");

                options.ClientId = googleOptions.ClientId!;
                options.ClientSecret = googleOptions.ClientSecret!;

                options.AccessType = "offline";

                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.ClaimActions.MapJsonKey("picture", "picture");
                options.ClaimActions.MapJsonKey("given_name", "given_name");
                options.ClaimActions.MapJsonKey("family_name", "family_name");

                //options.CallbackPath = "http://localhost:5000/auth/login/google/callback"; 
                options.ReturnUrlParameter = "/auth/login/google/callback";


            });



        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenWriterCookies, TokenWriterCookies>();
        services.AddSingleton<IEmailVerificationLinkFactory, EmailVerificationLinkFactory>();

        return services;
    }

    private static IServiceCollection AddAWS(this IServiceCollection services,
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
        // have its own polling and retry policies
        // TODO : recheck it 
        // SES 
        services.AddAWSService<IAmazonSimpleEmailService>();

        // S3 
        services.AddAWSService<IAmazonS3>();

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
        services.AddScoped<IVerificationEmailForRegistrationJob, VerificationEmailForRegistrationJob>();
        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();
        services.AddScoped<IOutboxCleanupJob, OutboxCleanupJob>();
        services.AddScoped<ITokenCleanupJob, TokenCleanupJob>();
        services.AddScoped<ISendingPasswordResetToken, SendingPasswordResetToken>();

        return services;

    }
    public static IServiceCollection AddResielenecPipelines(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResiliencePipeline(ProcessOutboxMessagesJob.OutboxProcessorPipelineKey,
            builder =>
            {
                // polly : add resilience for executing outbox messages
                builder.AddRetry(new RetryStrategyOptions
                {

                    Delay = TimeSpan.FromSeconds(1),
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true, // Helps prevent "thundering herd" issues.
                                      // This will add a random delay to each retry attempt, which can help distribute load more evenly.
                                      // to avoid overwhelming the system with retries at the same time.

                    OnRetry = args =>
                    {
                        Console.WriteLine($"Retrying operation due to: {args.Outcome.Exception?.Message}. Attempt #{args.AttemptNumber}");
                        return ValueTask.CompletedTask;
                    }
                });

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    FailureRatio = 0.5, // Break the circuit if 50% of requests fail...
                    MinimumThroughput = 5, // at least 5 request must be made before ( 1 request = complete process with retry )
                    SamplingDuration = TimeSpan.FromSeconds(60), // within a 60-second window.
                    BreakDuration = TimeSpan.FromSeconds(30),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"Circuit breaker opened for {args.BreakDuration.TotalSeconds}s due to: {args.Outcome.Exception?.Message}");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = _ =>
                    {
                        Console.WriteLine("Circuit breaker closed. Operations have resumed.");
                        return ValueTask.CompletedTask;
                    }
                });
            });

        return services;
    }


}
