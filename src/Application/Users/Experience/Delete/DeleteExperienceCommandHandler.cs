using System.Linq;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Experience.Delete;

internal sealed class DeleteExperienceCommandHandler(
   IApplicationDbContext context,
   ILogger<DeleteExperienceCommandHandler> logger) : ICommandHandler<DeleteExperienceCommand>
{
    public async Task<Result> Handle(DeleteExperienceCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling DeleteExperienceCommand for ExperienceId: {ExperienceId}", command.ExperienceId);

        var experience = await context.Experiences
            .Where(e => e.Id == command.ExperienceId && e.UserId == command.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (experience == null)
        {
            logger.LogWarning("Experience with ID: {ExperienceId} not found for user {UserId}", command.ExperienceId, command.UserId);
            return Result.Failure(ExperienceErrors.ExperienceNotFound);
        }

        context.Experiences.Remove(experience);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully deleted experience with ID: {ExperienceId}", experience.Id);
        return Result.Success();
    }
}