using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.TokenCleanup;

public interface IOutboxCleanupJob
{
    Task CleanUpAsync(PerformContext? context);
}