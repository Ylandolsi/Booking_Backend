using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Options;
using Domain.Users;
using Domain.Users.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.Login;

public sealed class LoginUserCommandHandler
                (IApplicationDbContext context,
                 IPasswordHasher passwordHasher,
                 ITokenProvider tokenProvider,
                 ITokenWriterCookies tokenWriterCookies,
                 IOptions<JwtOptions> jwtOptions,
                 ILogger<LoginUserCommandHandler> logger) : ICommandHandler<LoginUserCommand, Guid> 
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value; 

    public async Task<Result<Guid>> Handle(LoginUserCommand command, 
                         CancellationToken cancellationToken = default)
    {
        // check if email exists :
        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress.Email == command.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            logger.LogWarning("Login attempt failed for email : {Email}", command.Email);
            return Result.Failure<Guid>(UserErrors.IncorrectEmailOrPassword);
        }

        string accessToken = tokenProvider.GenrateJwtToken(user);
        if (string.IsNullOrEmpty(accessToken))
        {
            logger.LogError("Failed to generate access token for user with email: {Email}", command.Email);
            return Result.Failure<Guid>(UserErrors.TokenGenerationFailed);
        }

        string refreshTokenString = tokenProvider.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshTokenString))
        {
            logger.LogError("Failed to generate refresh token for user with email: {Email}", command.Email);
            return Result.Failure<Guid>(UserErrors.TokenGenerationFailed);
        }

       
        var refreshTokenEntity = new RefreshToken(
            refreshTokenString,
            user.Id,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));

        context.RefreshTokens.Add(refreshTokenEntity);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to save refresh token for user {UserId}", user.Id);
            return Result.Failure<Guid>(DatabaseErrors.SaveChangeError("Failed to save refresh token."));
        }


        logger.LogInformation("User {Email} logged in successfully. Refresh token generated and saved.", command.Email);

        tokenWriterCookies.WriteAccessTokenAsHttpOnlyCookie(accessToken);
        tokenWriterCookies.WriteRefreshTokenAsHttpOnlyCookie(refreshTokenString);


        return Result.Success(user.Id);
    }
}
