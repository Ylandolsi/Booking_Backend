using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Authentication.ChangePassword;

public static class ChangePasswordErrors
{
    public static readonly Error PasswordIncorrect = Error.Problem(
        "ChangePassword.PasswordIncorrect", "The old password is not correct "
    );
    public static readonly Error PasswordsDoNotMatch = Error.Problem(
        "ChangePassword.PasswordsDoNotMatch",
        "The new password and confirmation password do not match.");

    public static Error ChangePasswordFailed(string errors) => Error.Failure(
        "ChangePassword.Failed",
        $"Failed to change password: {errors}");
}

internal sealed class ChangePasswordCommandHandler(UserManager<User> userManager,
                                                   ILogger<ChangePasswordCommandHandler> logger)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to change password for user {UserId}", command.UserId);

        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
        {
            logger.LogWarning("Password change failed: User with ID {UserId} not found.", command.UserId);
            return Result.Failure(UserErrors.NotFoundById(command.UserId));
        }
        if (!await userManager.CheckPasswordAsync(user, command.OldPassword))
        {
            logger.LogWarning("Old password is wrong");
            return Result.Failure(ChangePasswordErrors.PasswordIncorrect);
        }

        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Password change failed for user {UserId}. Errors: {Errors}", command.UserId, errorMessages);
            return Result.Failure(ChangePasswordErrors.ChangePasswordFailed(errorMessages));
        }

        logger.LogInformation("Password successfully changed for user {UserId}", command.UserId);
        return Result.Success();
    }
}