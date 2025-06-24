using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Web;

namespace Application.Users.Authentication.ResetPassword.Verify;

internal sealed class VerifyResetPasswordCommandHandler(UserManager<User> userManager,
                                                           ILogger<VerifyResetPasswordCommandHandler> logger) : ICommandHandler<VerifyResetPasswordCommand>
{
    public async Task<Result> Handle(VerifyResetPasswordCommand command, CancellationToken cancellationToken)
    {
        // decode the email and token if they are URL encoded
        var updatedCommand = new VerifyResetPasswordCommand(
            HttpUtility.UrlDecode(command.Email),
            HttpUtility.UrlDecode(command.Token),
            command.Password,
            command.ConfirmPassword
        );


        logger.LogInformation("Handling VerifyResetPasswordCommand for email: {Email}", updatedCommand.Email);
        User? user = await userManager.FindByEmailAsync(updatedCommand.Email);
        if (user is null)
        {
            // dont reveal if user exists or not
            await SimulatePasswordResetWorkAsync();
        }
        else
        {
            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                // if the email is not confirmed , confirm it before resetting password
                var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await userManager.ConfirmEmailAsync(user, emailConfirmToken);
            }

            IdentityResult? resetPasswordResult = await userManager.ResetPasswordAsync(user!, updatedCommand.Token, updatedCommand.Password);
            if (!resetPasswordResult.Succeeded)
            {
                logger.LogWarning("Failed to reset password for user with email {Email}. Errors: {Errors}",
                    updatedCommand.Email,
                    string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description)));
                return Result.Failure(ResetPasswordErrors.GenericError);
            }

        }

        logger.LogInformation("Password reset successfully for user with email {Email}", updatedCommand.Email);
        return Result.Success();

    }
    private static async Task SimulatePasswordResetWorkAsync()
    {
        var delay = Random.Shared.Next(150, 250);
        await Task.Delay(delay);
    }
}

