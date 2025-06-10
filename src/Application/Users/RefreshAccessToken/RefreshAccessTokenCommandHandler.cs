using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Options;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;


namespace Application.Users.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler(IApplicationDbContext applicationDbContext , 
                                                     ITokenProvider tokenProvider , 
                                                     ITokenWriterCookies tokenWriterCookies,
                                                     ILogger<RefreshAccessTokenCommandHandler> logger,
                                                     IOptions<JwtOptions> jwtOptions) : ICommandHandler<RefreshAccessTokenCommand, bool>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<bool>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await applicationDbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken); 
        if (refreshToken == null)
        {
            return Result.Failure<bool>(
                Error.Unauthorized("RefreshToken.Invalid", "The provided refresh token is invalid or does not exist."));
        }

        if (refreshToken.IsRevoked)
        {
            logger.LogWarning(
                "Security Alert: Attempt to use a revoked refresh token for User ID: {UserId}. Invalidating all other active refresh tokens for this user.",
                refreshToken.UserId);

            // Invalidate all other active refresh tokens for this user
            var otherActiveTokens = await applicationDbContext.RefreshTokens
                .Where(rt => rt.UserId == refreshToken.UserId && !rt.IsRevoked && rt.ExpiresOnUtc > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (otherActiveTokens.Any())
            {
                foreach (var tokenToRevoke in otherActiveTokens)
                {
                    tokenToRevoke.Revoke(); 
                }
                await applicationDbContext.SaveChangesAsync(cancellationToken); 
                logger.LogInformation("Successfully invalidated {Count} other active refresh token(s) for User ID: {UserId}.", otherActiveTokens.Count, refreshToken.UserId);
            }
            return Result.Failure<bool>(
                Error.Unauthorized("RefreshToken.Revoked", "The provided refresh token has been revoked."));
        }

        if (refreshToken.IsExpired)
        {
            return Result.Failure<bool>(
                Error.Unauthorized("RefreshToken.Expired", "The provided refresh token has expired."));
        }

        string jwtAccessToken = tokenProvider.GenrateJwtToken(refreshToken.User);
        string jwtRefreshToken = tokenProvider.GenerateRefreshToken();

        refreshToken.Revoke();
        var refreshTokenEntity = new RefreshToken(
                jwtRefreshToken,
                refreshToken.UserId,
                DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        await applicationDbContext.RefreshTokens.AddAsync(refreshTokenEntity);
        try
        {
            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            return Result.Failure<bool>(
                Error.Failure("Database.SaveChanges", "Failed to save refresh token."));
        }

        tokenWriterCookies.WriteRefreshTokenAsHttpOnlyCookie(jwtRefreshToken);
        tokenWriterCookies.WriteAccessTokenAsHttpOnlyCookie(jwtAccessToken);



        return true; 






    }
}