using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Web;

namespace Application.Users.Authentication.Verification.VerifyEmail;


public class VerifyEmailCommandHandler(UserManager<User> userManager,
                                       ILogger<VerifyEmailCommandHandler> logger) : ICommandHandler<VerifyEmailCommand>
{
    public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        // they are decoded by react !

        logger.LogInformation("Handling VerifyEmailCommand for email: {Email}", command.Email) ;

        User? user = await userManager.FindByEmailAsync(command.Email) ;
        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", command.Email) ;
            return Result.Failure(VerifyEmailErrors.EmailOrTokenInvalid);
        }
        if (user.EmailConfirmed)
        {
            logger.LogInformation("Email for user with email: {Email} is already confirmed", command.Email) ;
            return Result.Failure(VerifyEmailErrors.AlreadyVerified);
        }


        IdentityResult result = await userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to confirm email for user with email: {Email}. Errors: {Errors}", command.Email, result.Errors);
            return Result.Failure(VerifyEmailErrors.TokenExpired);

        }

        logger.LogInformation("Email confirmed successfully for user with email: {Email}", command.Email);

        return Result.Success();
    }

}