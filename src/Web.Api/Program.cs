using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using Web.Api;
using Web.Api.Extensions;
using Hangfire;
using Web.Api.RecurringJobs;
using Microsoft.AspNetCore.Identity;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEmailSender(builder.Configuration);


builder.Services.UseHangFire(builder.Configuration);



builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTitle("Booking API");
    });
    app.ApplyMigrations();

    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Delete test users only
    var testUsers = await userManager.Users
        .ToListAsync();

    foreach (var user in testUsers)
    {
        await userManager.DeleteAsync(user);
    }
    var testUser = User.Create("Test",
                               "User",
                               "yesslandolsi@gmail.com",
                               "");

    var result = await userManager.CreateAsync(testUser, "Password123!");

    if (result.Succeeded)
    {
        var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(testUser);
        await userManager.ConfirmEmailAsync(testUser, emailToken);

        Console.WriteLine("✅ Seeded verified user: yesslandolsi@gmail.com/ Password123!");
    }
    else
    {
        Console.WriteLine("❌ Failed to seed user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// TODO  : app.useStaticfiles ? 

app.UseCors("DefaultCors"); 
app.UseHangfireDashboard();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseCancellationMiddleware();

RecurringJobs.UseTokenCleanup();
RecurringJobs.UseOutboxMessagesCleanUp();
RecurringJobs.UseOutboxMessgesProcessor();


app.UseAuthentication();

app.UseAuthorization();


await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public partial class Program;
}