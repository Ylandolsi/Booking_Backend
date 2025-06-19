using Application.Abstractions.BackgroundJobs.TokenCleanup;
using Application.Abstractions.Data;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundJobs.TokenCleanup;

public class TokenCleanupJob : ITokenCleanupJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<TokenCleanupJob> _logger;

    public TokenCleanupJob(IApplicationDbContext context, ILogger<TokenCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [DisplayName("Clean up expired revoked/expired refresh tokens")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CleanUpAsync(PerformContext? context)
    {
        context?.WriteLine("Starting token cleanup job...");
        _logger.LogInformation("Hangfire Job: Starting token cleanup job...");

        // provided by the background job server ( eg hangfire)
        // to shutdown gracefully 
        var cancellationToken = context?.CancellationToken.ShutdownToken ?? CancellationToken.None;

        var utcNow = DateTime.UtcNow;
        int refreshTokensRemovedCount = 0;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();


            // Remove revoked or expired refresh tokens
            var oldRefreshTokens = await _context.RefreshTokens
                .Where(rt => rt.RevokedOnUtc.HasValue || rt.ExpiresOnUtc < utcNow)
                .ToListAsync();

            if (oldRefreshTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(oldRefreshTokens);
                refreshTokensRemovedCount = oldRefreshTokens.Count;
                context?.WriteLine($"Found {refreshTokensRemovedCount} revoked or expired refresh tokens to remove.");
                _logger.LogInformation("Hangfire Job: Found {Count} revoked or expired refresh tokens to remove.", refreshTokensRemovedCount);
            }
            else
            {
                context?.WriteLine("No revoked or expired refresh tokens found.");
                _logger.LogInformation("Hangfire Job: No revoked or expired refresh tokens found.");
            }

            if (refreshTokensRemovedCount > 0)
            {
                await _context.SaveChangesAsync(CancellationToken.None); // Assuming CancellationToken.None is acceptable for a background job
                context?.WriteLine("Successfully removed tokens from the database.");
                _logger.LogInformation("Hangfire Job: Successfully removed {RefreshTokenCount} refresh tokens.", refreshTokensRemovedCount);
            }
        }
        catch (OperationCanceledException)
        {
            // The job was gracefully canceled during a server shutdown.
            context?.WriteLine("Token cleanup job was canceled.");
            _logger.LogWarning("Hangfire Job: Token cleanup job was canceled during shutdown.");
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Error during token cleanup: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: Error occurred during token cleanup.");
            throw; // Re-throw to allow Hangfire to handle retries
        }

        context?.WriteLine("Token cleanup job finished.");
        _logger.LogInformation("Hangfire Job: Token cleanup job finished. Removed {RefreshTokenCount} refresh tokens.", refreshTokensRemovedCount);
    }
}