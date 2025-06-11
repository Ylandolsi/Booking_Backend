using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Experience.Update;

internal sealed class UpdateExperienceCommandHandler(
    IApplicationDbContext context,
    ILogger<UpdateExperienceCommandHandler> logger) : ICommandHandler<UpdateExperienceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateExperienceCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to update experience with ID: {ExperienceId}", command.UserId);
        var experience = await context.Experiences.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

        if (experience is null)
        {
            logger.LogWarning("Experience with ID: {ExperienceId} not found for update.", command.UserId);
            return Result.Failure<Guid>(ExperienceErrors.ExperienceNotFound);
        }


        var updateResult = experience.Update(
            command.Title,
            command.Company,
            command.StartDate,
            command.EndDate,
            command.Description);

        if (updateResult.IsFailure)
        {
            logger.LogWarning("Failed to update experience with ID: {ExperienceId}. Error: {Error}", command.UserId, updateResult.Error);
            return Result.Failure<Guid>(updateResult.Error);
        }
         context.Experiences.Update(experience); 

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully updated experience with ID: {ExperienceId}", experience.Id);
        return Result.Success(experience.Id);
    }
}
