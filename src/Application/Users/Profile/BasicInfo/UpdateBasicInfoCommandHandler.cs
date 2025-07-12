using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Profile.BasicInfo;


internal sealed class UpdateBasicInfoCommandHandler(
    UserManager<User> userManager,
    ILogger<UpdateBasicInfoCommandHandler> logger) : ICommandHandler<UpdateBasicInfoCommand>
{
    public async Task<Result> Handle(UpdateBasicInfoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating basic info for user {UserId}", command.UserId);

        User? user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure(UserErrors.NotFoundById(command.UserId));
        }

        user.UpdateName(command.FirstName, command.LastName);
        if (command.Bio != null) user.UpdateBio(command.Bio);
        user.UpdateGender(command.Gender);

        user.ProfileCompletionStatus.UpdateCompletionStatus(user);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to update basic info for user {UserId}. Errors: {Errors}",
                command.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result.Failure(Error.Problem("User.UpdateFailed", "Failed to update user"));
        }

        logger.LogInformation("Successfully updated basic info for user {UserId}", command.UserId);
        return Result.Success();
    }
}