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

    [DisplayName("Clean up expired verification tokens and revoked/expired refresh tokens")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CleanUpAsync(PerformContext? context)
    {
        context?.WriteLine("Starting token cleanup job...");
        _logger.LogInformation("Hangfire Job: Starting token cleanup job...");

        var utcNow = DateTime.UtcNow;
        int emailTokensRemovedCount = 0;
        int refreshTokensRemovedCount = 0;

        try
        {
            // Remove expired email verification tokens
            var expiredEmailVerificationTokens = await _context.EmailVerificationTokens
                .Where(t => t.ExpiresOnUtc < utcNow)
                .ToListAsync();

            if (expiredEmailVerificationTokens.Any())
            {
                _context.EmailVerificationTokens.RemoveRange(expiredEmailVerificationTokens);
                emailTokensRemovedCount = expiredEmailVerificationTokens.Count;
                context?.WriteLine($"Found {emailTokensRemovedCount} expired email verification tokens to remove.");
                _logger.LogInformation("Hangfire Job: Found {Count} expired email verification tokens to remove.", emailTokensRemovedCount);
            }
            else
            {
                context?.WriteLine("No expired email verification tokens found.");
                _logger.LogInformation("Hangfire Job: No expired email verification tokens found.");
            }

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

            if (emailTokensRemovedCount > 0 || refreshTokensRemovedCount > 0)
            {
                await _context.SaveChangesAsync(CancellationToken.None); // Assuming CancellationToken.None is acceptable for a background job
                context?.WriteLine("Successfully removed tokens from the database.");
                _logger.LogInformation("Hangfire Job: Successfully removed {EmailTokenCount} email tokens and {RefreshTokenCount} refresh tokens.", emailTokensRemovedCount, refreshTokensRemovedCount);
            }
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Error during token cleanup: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: Error occurred during token cleanup.");
            throw; // Re-throw to allow Hangfire to handle retries
        }

        context?.WriteLine("Token cleanup job finished.");
        _logger.LogInformation("Hangfire Job: Token cleanup job finished. Removed {EmailTokenCount} email tokens and {RefreshTokenCount} refresh tokens.", emailTokensRemovedCount, refreshTokensRemovedCount);
    }
}