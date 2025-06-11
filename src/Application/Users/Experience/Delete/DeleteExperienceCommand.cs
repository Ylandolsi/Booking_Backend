using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Hangfire.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Experience.Delete;

public sealed record DeleteExperienceCommand (Guid ExperienceId , Guid UserId ) : ICommand<Guid>;

internal sealed class DeleteExperienceCommandHandler (
    IApplicationDbContext context,
    ILogger<DeleteExperienceCommandHandler> logger) : ICommandHandler<DeleteExperienceCommand, Guid>
{

    public async Task<Result<Guid>> Handle(DeleteExperienceCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling DeleteExperienceCommand for ExperienceId: {ExperienceId}", command.ExperienceId);
        Domain.Users.Entities.Experience?  experience = await context.Experiences.FirstOrDefaultAsync(
            x => x.Id == command.ExperienceId && x.UserId == command.UserId, cancellationToken);
        
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


