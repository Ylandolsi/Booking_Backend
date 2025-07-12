using Application.Abstractions.Messaging;
using Application.Abstractions.Uploads;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Profile.ProfilePicture;

internal sealed class UpdateProfilePictureCommandHandler(UserManager<User> userManager,
                                                         IS3ImageProcessingService imageProcessingService,
                                                         ILogger<UpdateProfilePictureCommandHandler> logger) : ICommandHandler<UpdateProfilePictureCommand, string>
{
    public async Task<Result<string>> Handle(UpdateProfilePictureCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating profile picture for user {UserId}", command.UserId);

        User? user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure<string>(UserErrors.NotFoundById(command.UserId));
        }

        var fileName = $"profile_{command.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        var imageResult = await imageProcessingService.ProcessImageAsync(command.File, fileName);

        user.ProfilePictureUrl.UpdateProfilePicture(imageResult.OriginalUrl, imageResult.ThumbnailUrl);

        user.ProfileCompletionStatus.UpdateCompletionStatus(user);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to update profile picture for user {UserId}. Errors: {Errors}",
                command.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result.Failure<string>(Error.Problem("User.UpdateFailed", "Failed to update profile picture"));
        }

        logger.LogInformation("Successfully updated profile picture for user {UserId}", command.UserId);
        return Result.Success(imageResult.OriginalUrl);
    }
}