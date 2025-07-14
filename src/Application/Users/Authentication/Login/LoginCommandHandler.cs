using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Options;
using Application.Users.Authentication;
using Application.Users.Authentication.Utils;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
                 TokenHelper tokenHelper,
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



        string? currentIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        string? currentUserAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        Result resultt = await tokenHelper.GenerateTokens(user, currentIp, currentUserAgent, cancellationToken);
        if (resultt.IsFailure)
        {
            return Result.Failure<LoginResponse>(resultt.Error);
        }
        logger.LogInformation("User {Email} logged in successfully.!", command.Email);


        var response = new LoginResponse
        (
            UserSlug: user.Slug,
            FirstName: user.Name.FirstName,
            LastName: user.Name.LastName,
            Email: user.Email!,
            ProfilePictureUrl: user.ProfilePictureUrl.ProfilePictureLink,
            IsMentor: user.Status.IsMentor,
            MentorActive: user.Status.IsMentor && user.Status.IsActive
        );


        return Result.Success(response);
    }
}


