using Application.Abstractions.BackgroundJobs.TokenCleanup;
using Application.Abstractions.Data;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;


namespace Infrastructure.BackgroundJobs.Cleanup;

public class OutboxCleanupJob : IOutboxCleanupJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<OutboxCleanupJob> _logger;

    public OutboxCleanupJob(IApplicationDbContext context, ILogger<OutboxCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [DisplayName("Clean up processed outbox messages")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CleanUpAsync(PerformContext? context)
    {
        context?.WriteLine("Starting outbox cleanup job...");
        _logger.LogInformation("Hangfire Job: Starting outbox cleanup job...");

        var cancellationToken = context?.CancellationToken.ShutdownToken ?? CancellationToken.None;

        var utcNow = DateTime.UtcNow;
        int outboxMessagesRemovedCount = 0;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();


            var thresholdDate = utcNow.AddDays(-2);
            var processedOutboxMessages = await _context.OutboxMessages
                .Where(om => om.ProcessedOnUtc.HasValue && om.ProcessedOnUtc < thresholdDate)
                .ToListAsync(cancellationToken);

            if (processedOutboxMessages.Any())
            {
                _context.OutboxMessages.RemoveRange(processedOutboxMessages);
                outboxMessagesRemovedCount = processedOutboxMessages.Count;
                context?.WriteLine($"Found {outboxMessagesRemovedCount} processed outbox messages to remove.");
                _logger.LogInformation("Hangfire Job: Found {Count} processed outbox messages to remove.", outboxMessagesRemovedCount);
            }
            else
            {
                context?.WriteLine("No processed outbox messages found.");
                _logger.LogInformation("Hangfire Job: No processed outbox messages found.");
            }

            if (outboxMessagesRemovedCount > 0)
            {
                await _context.SaveChangesAsync(CancellationToken.None); // Assuming CancellationToken.None is acceptable for a background job
                context?.WriteLine("Successfully removed processed outbox messages from the database.");
                _logger.LogInformation("Hangfire Job: Successfully removed {OutboxMessageCount} processed outbox messages.", outboxMessagesRemovedCount);
            }
        }
        catch (OperationCanceledException)
        {
            context?.WriteLine("Outbox cleanup job was canceled.");
            _logger.LogWarning("Hangfire Job: Outbox cleanup job was canceled during shutdown.");
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Error during outbox cleanup: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: Error occurred during outbox cleanup.");
            throw; // Re-throw to allow Hangfire to handle retries
        }

        context?.WriteLine("Outbox cleanup job finished.");
        _logger.LogInformation("Hangfire Job: Outbox cleanup job finished. Removed {OutboxMessageCount} processed outbox messages.", outboxMessagesRemovedCount);
    }
}

