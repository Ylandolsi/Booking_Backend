using Hangfire;
using Application.Abstractions.BackgroundJobs.TokenCleanup;
namespace Web.Api.RecurringJobs;

public static class RecurringJobs
{
    public static void UseTokenCleanup()
    {
        RecurringJob.AddOrUpdate<ITokenCleanupJob>(

            "token-cleanup-job",
            job => job.CleanUpAsync(null),
            Cron.Daily(1, 0), // Runs daily at 2:00 AM GMT+1 
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc // timezone = GMT (UTC)
            });

    }
}