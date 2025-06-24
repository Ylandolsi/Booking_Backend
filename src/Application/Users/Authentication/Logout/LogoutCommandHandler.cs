using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Authentication.Logout;


public static class LogoutErrors
{
    public static readonly Error NoActiveSession = Error.Problem("No.Refresh.Token", "No active session found for the user.");
}


internal sealed class LogoutCommandHandler(IApplicationDbContext context,
                                         IUserContext userContext,
                                         ITokenWriterCookies tokenWriterCookies,
                                         ILogger<LogoutCommandHandler> logger) : ICommandHandler<LogoutCommand, bool>
{

    public async Task<Result<bool>> Handle(LogoutCommand query, CancellationToken cancellationToken)
    {
        string? currentRefreshToken = userContext.RefreshToken;
        if (string.IsNullOrEmpty(currentRefreshToken))
        {
            logger.LogWarning("No refresh token found for user ID: {UserId}", query.UserId);
            return Result.Failure<bool>(LogoutErrors.NoActiveSession);
        }

        logger.LogInformation("Logging out user ID: {UserId}", query.UserId);

        tokenWriterCookies.ClearRefreshTokenCookie();
        tokenWriterCookies.ClearAccessTokenCookie();
        logger.LogInformation("Cleared tokens cookie for user ID: {UserId}", query.UserId);

        // revoke it in the database 

        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == currentRefreshToken && rt.UserId == query.UserId, cancellationToken);

        if (refreshToken is null)
        {
            logger.LogWarning("Refresh token not found for user ID: {UserId}", query.UserId);
            return Result.Failure<bool>(LogoutErrors.NoActiveSession);
        }

        refreshToken.Revoke();

        context.RefreshTokens.Update(refreshToken);

        return true;

    }
}
