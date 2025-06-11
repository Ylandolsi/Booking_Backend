using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Experience.Delete;

internal sealed class DeleteExperienceCommandHandler (
    IApplicationDbContext context,
    ILogger<DeleteExperienceCommandHandler> logger) : ICommandHandler<DeleteExperienceCommand, Guid>
{

    public async Task<Result<Guid>> Handle(DeleteExperienceCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling DeleteExperienceCommand for ExperienceId: {ExperienceId}", command.ExperienceId);
        Domain.Users.Entities.Experience experience = await context.Experiences.FindAsync(new object[] { command.ExperienceId }, cancellationToken);
        
        if (experience == null)
        {
            logger.LogWarning("Experience with ID: {ExperienceId} not found for deletion.", command.ExperienceId);
            return Result.Failure<Guid>(ExperienceErrors.ExperienceNotFound); 
        }
        context.Experiences.Remove(experience);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully deleted experience with ID: {ExperienceId}", experience.Id);    
        return experience.Id;
    }
}


