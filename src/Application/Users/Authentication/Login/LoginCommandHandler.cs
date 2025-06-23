using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Options;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Application.Users.Login;

public sealed class LoginCommandHandler
                (IApplicationDbContext context,
                 UserManager<User> userManager,
                 ITokenProvider tokenProvider,
                 ITokenWriterCookies tokenWriterCookies,
                 IHttpContextAccessor httpContextAccessor,
                 IOptions<JwtOptions> jwtOptions,
                 ILogger<LoginCommandHandler> logger) : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly AccessOptions _jwtOptions = jwtOptions.Value.AccessToken;

    public async Task<Result<LoginResponse>> Handle(LoginCommand command,
                                                        CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            logger.LogWarning("Login attempt failed for email : {Email}", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.IncorrectEmailOrPassword);
        }
        if (await userManager.IsLockedOutAsync(user))
        {
            logger.LogWarning("Login attempt for locked-out account: {Email}", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.AccountLockedOut);
        }

        if (string.IsNullOrEmpty(command.Password) || !await userManager.CheckPasswordAsync(user, command.Password))
        {
            logger.LogWarning("Login attempt failed for email: {Email} - Incorrect password", command.Email);
            await userManager.AccessFailedAsync(user); // increment failed access count
            return Result.Failure<LoginResponse>(UserErrors.IncorrectEmailOrPassword);
        }

        // if succefully logged in , reset the failed access count
        await userManager.ResetAccessFailedCountAsync(user);

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("Login attempt failed for email: {Email} - Email not confirmed", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.EmailIsNotVerified);
        }

        string accessToken = tokenProvider.GenrateJwtToken(user);
        if (string.IsNullOrEmpty(accessToken))
        {
            logger.LogError("Failed to generate access token for user with email: {Email}", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.TokenGenerationFailed);
        }

        string refreshToken = tokenProvider.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogError("Failed to generate refresh token for user with email: {Email}", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.TokenGenerationFailed);
        }
        string? currentIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        string? currentUserAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

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
            return Result.Failure<LoginResponse>(DatabaseErrors.SaveChangeError("Failed to save refresh token."));
        }


        logger.LogInformation("User {Email} logged in successfully. Refresh token generated and saved.", command.Email);

        tokenWriterCookies.WriteRefreshTokenAsHttpOnlyCookie(refreshToken);


        var response = new LoginResponse
        (
            UserId: user.Id,
            AccessToken: accessToken,
            Firstname: user.Name.FirstName,
            Lastname: user.Name.LastName,
            Email: user.Email!,
            ProfilePictureUrl: user.ProfilePictureUrl.ProfilePictureLink,
            IsMentor: user.Status.IsMentor,
            MentorActive: user.Status.IsMentor && user.Status.IsActive
        );


        return Result.Success(response);
    }
}
