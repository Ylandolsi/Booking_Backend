using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Experience.Add;

internal sealed class AddExperienceCommandHandler(IApplicationDbContext context,
                                                  ILogger<AddExperienceCommandHandler> logger) : ICommandHandler<AddExperienceCommand, int>
{
    public async Task<Result<int>> Handle(AddExperienceCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding experience for user {UserId}", command.UserId);

        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure<int>(UserErrors.NotFoundById(command.UserId));
        }

        var experience = new Domain.Users.Entities.Experience(
            command.Title,
            command.Description ?? string.Empty,
            command.Company,
            command.UserId,
            command.StartDate,
            command.EndDate
        );

        try
        {
            await context.Experiences.AddAsync(experience, cancellationToken);
            user.ProfileCompletionStatus.UpdateCompletionStatus(user);
            
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add experience for user {UserId}", command.UserId);
            return Result.Failure<int>(Error.Problem("Experience.AddFailed", "Failed to add experience"));
        }

        logger.LogInformation("Successfully added experience {ExperienceId} for user {UserId}",
            experience.Id, command.UserId);
        return Result.Success(experience.Id);
    }
}