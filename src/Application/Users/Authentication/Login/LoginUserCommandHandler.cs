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
                 ILogger<LoginUserCommandHandler> logger) : ICommandHandler<LoginUserCommand, LoginUserResponse> 
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value; 

    public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand command, 
                         CancellationToken cancellationToken = default)
    {
        // check if email exists :
        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress.Email == command.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            logger.LogWarning("Login attempt failed for email : {Email}", command.Email);
            return Result.Failure<LoginUserResponse>(UserErrors.IncorrectEmailOrPassword);
        }

        if (!user.EmailAddress.IsVerified())
        {
            logger.LogWarning("User with email {Email} has not verified their email address.", command.Email);
            return Result.Failure<LoginUserResponse>(UserErrors.EmailIsNotVerified);
        }

        string accessToken = tokenProvider.GenrateJwtToken(user);
        if (string.IsNullOrEmpty(accessToken))
        {
            logger.LogError("Failed to generate access token for user with email: {Email}", command.Email);
            return Result.Failure<LoginUserResponse>(UserErrors.TokenGenerationFailed);
        }

        string refreshToken = tokenProvider.GenerateRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogError("Failed to generate refresh token for user with email: {Email}", command.Email);
            return Result.Failure<LoginUserResponse>(UserErrors.TokenGenerationFailed);
        }

       
        var refreshTokenEntity = new RefreshToken(
            refreshToken,
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
            return Result.Failure<LoginUserResponse>(DatabaseErrors.SaveChangeError("Failed to save refresh token."));
        }


        logger.LogInformation("User {Email} logged in successfully. Refresh token generated and saved.", command.Email);

        tokenWriterCookies.WriteRefreshTokenAsHttpOnlyCookie(refreshToken);


        var response = new LoginUserResponse
        (
            UserId: user.Id,
            AccessToken: accessToken,
            Firstname: user.Name.FirstName,
            Lastname: user.Name.LastName,
            Email: user.EmailAddress.Email,
            ProfilePictureUrl: user.ProfilePictureUrl.ProfilePictureLink,
            IsMentor: user.Status.IsMentor,
            MentorActive: user.Status.IsMentor && user.Status.IsActive
        ); 


        return Result.Success(response);
    }
}
