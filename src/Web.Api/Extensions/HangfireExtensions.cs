using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using Web.Api.Middleware;

namespace Web.Api.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection UseHangFire(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConnectionString = configuration.GetConnectionString("Database");

        services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer() // Store only namespace and assembly name of the job 
               .UseRecommendedSerializerSettings()
               .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(hangfireConnectionString), new PostgreSqlStorageOptions
               {
                   SchemaName = "hangfire"
               })
               .UseConsole()); // logs to the Hangfire dashboard

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount; // = cpu cores
            options.Queues = ["default", "critical"];
        });


        return services;


    }

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app)
    {
        // Add Hangfire Dashboard
        //  secure this dashboard in production
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [] // Anyone can access the dashboard (for development)
                               // In production, you'd implement IAuthorizationFilter, e.g.:
                               // Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });
            
        return app;

    }
}