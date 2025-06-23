using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Authentication.Verification.VerifyEmail;

public class VerifyEmailCommandHandler(UserManager<User> userManager,
                                       ILogger<VerifyEmailCommandHandler> logger) : ICommandHandler<VerifyEmailCommand>
{
    public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        // decode the token from the command cuz its URL encoded
        string decodedToken = System.Net.WebUtility.UrlDecode(command.Token);
        string decodedEmail = System.Net.WebUtility.UrlDecode(command.Email);


        logger.LogInformation("Handling VerifyEmailCommand for email: {Email}", decodedEmail) ;

        User? user = await userManager.FindByEmailAsync(decodedEmail) ;
        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", decodedEmail) ;
            return Result.Failure(UserErrors.NotFoundByEmail(decodedEmail) );
        }
        if (user.EmailConfirmed)
        {
            logger.LogInformation("Email for user with email: {Email} is already confirmed", decodedEmail) ;
            return Result.Failure(VerifyEmailErrors.AlreadyVerified);
        }




        IdentityResult result = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to confirm email for user with email: {Email}. Errors: {Errors}", decodedEmail, result.Errors);
            return Result.Failure(VerifyEmailErrors.TokenExpired);

        }

        logger.LogInformation("Email confirmed successfully for user with email: {Email}", decodedEmail);

        return Result.Success();
    }

}