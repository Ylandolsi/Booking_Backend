using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs;

public interface IProcessOutboxMessagesJob
{
    Task ExecuteAsync(PerformContext? context);
}