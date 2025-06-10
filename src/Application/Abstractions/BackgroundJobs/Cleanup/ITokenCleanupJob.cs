using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.TokenCleanup;

public interface ITokenCleanupJob
{
    Task CleanUpAsync(PerformContext? context);
}

