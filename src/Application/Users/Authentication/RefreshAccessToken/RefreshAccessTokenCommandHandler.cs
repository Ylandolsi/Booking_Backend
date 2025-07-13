using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Options;
using Application.Users.Authentication;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;


namespace Application.Users.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler(IApplicationDbContext applicationDbContext,
                                                     ITokenProvider tokenProvider,
                                                     UserManager<User> userManager,
                                                     TokenHelper tokenHelper , 
                                                     ITokenWriterCookies tokenWriterCookies,
                                                     IHttpContextAccessor httpContextAccessor,
                                                     ILogger<RefreshAccessTokenCommandHandler> logger,
                                                     IOptions<JwtOptions> jwtOptions) : ICommandHandler<RefreshAccessTokenCommand>
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public async Task<Result> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        
       var refreshToken = await applicationDbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);
        if (refreshToken == null)
        {
            return Result.Failure<string>(RefreshTokenErrors.Unauthorized);
        }


        var activeTokenCount = await applicationDbContext.RefreshTokens
            .CountAsync(rt => rt.UserId == refreshToken.UserId && rt.RevokedOnUtc == null  && rt.ExpiresOnUtc > DateTime.UtcNow, cancellationToken);

        if (activeTokenCount >= _jwtOptions.MaxActiveTokensPerUser)
        {
            logger.LogWarning(
                "User {UserId} has reached the maximum active refresh token limit of {MaxActiveTokens}. Revoking the oldest active token.",
                refreshToken.UserId, _jwtOptions.MaxActiveTokensPerUser);


            var oldestToken = await applicationDbContext.RefreshTokens
                .Where(rt => rt.UserId == refreshToken.UserId && rt.IsActive)
                .OrderBy(rt => rt.CreatedOnUtc)
                .FirstOrDefaultAsync(cancellationToken);

            if (oldestToken is not null)
            {
                oldestToken.Revoke();
                logger.LogInformation("User {UserId} has reached the active refresh token limit. The oldest token has been revoked.", refreshToken.UserId);
            }

        }
        if (refreshToken.IsRevoked)
        {
            logger.LogWarning(
                "SECURITY ALERT : Attempt to use a revoked refresh token for User ID: {UserId}. Invalidating all other active refresh tokens for this user.",
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
                // update stamp to invalidate the access tokens
                userManager.UpdateSecurityStampAsync(refreshToken.User).GetAwaiter().GetResult();
                await applicationDbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Successfully invalidated {Count} other active refresh token(s) for User ID: {UserId}.", otherActiveTokens.Count, refreshToken.UserId);
            }
            return Result.Failure<string>(RefreshTokenErrors.Revoked);
        }
        string? currentIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        string? currentUserAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        if (currentIp != refreshToken.CreatedByIp ||
            currentUserAgent != refreshToken.UserAgent)
        {
            logger.LogWarning("SECURITY INFO: Refresh token for User {UserId} used from a new IP. Original: {OriginalIp}, Current: {CurrentIp}.",
                              refreshToken.UserId,
                              refreshToken.CreatedByIp,
                              currentIp);
            // TODO : trigger security alert or log this event
            // TODO : send email notification to the user about this event

            // await _notificationService.SendNewIpLoginAlert(refreshToken.User.Email, currentIp);

        }

        if (refreshToken.IsExpired)
        {
            return Result.Failure<string>(RefreshTokenErrors.Expired);
        }



        refreshToken.Revoke();

        Result result = await tokenHelper.GenerateTokens(refreshToken.User, currentIp, currentUserAgent, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<string>(result.Error);
        }


        return Result.Success();

    }
}