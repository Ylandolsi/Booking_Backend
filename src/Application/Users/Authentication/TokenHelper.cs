using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Options;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;


namespace Application.Users.Authentication;

public static class TokenGenerationError
{
    public static readonly Error TokenGenerationFailed = Error.Failure(
        "Users.TokenGenerationFailed",
        "Failed to generate a token for the user. Please try again later.");
}

public class TokenHelper(ITokenProvider tokenProvider,
                           IApplicationDbContext context,
                           ITokenWriterCookies tokenWriterCookies,
                           IOptions<JwtOptions> jwtOptions,
                           ILogger<TokenHelper> logger)
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public async Task<Result> GenerateTokens(User user,
                                             string? currentIp,
                                             string? currentUserAgent,
                                             CancellationToken cancellationToken)
    {

        string accessToken = tokenProvider.GenerateJwtToken(user);
        if (string.IsNullOrEmpty(accessToken))
        {
            logger.LogError("Failed to generate access token for user with email: {Email}", user.Email);
            return Result.Failure<string>(TokenGenerationError.TokenGenerationFailed);
        }

        string refreshToken = tokenProvider.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogError("Failed to generate refresh token for user with email: {Email}", user.Email);
            return Result.Failure<string>(TokenGenerationError.TokenGenerationFailed);
        }


        var refreshTokenEntity = new RefreshToken(
            refreshToken,
            user.Id,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            currentIp,
            currentUserAgent
        );

        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to save refresh token for user {UserId}", user.Id);
            return Result.Failure<string>(DatabaseErrors.SaveChangeError("Failed to save refresh token."));
        }


        logger.LogInformation("Refresh token generated and saved.", user.Email);

        tokenWriterCookies.WriteRefreshTokenAsHttpOnlyCookie(refreshToken);
        tokenWriterCookies.WriteAccessTokenAsHttpOnlyCookie(accessToken);


        return Result.Success();

    }
}
