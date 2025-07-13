using System.Linq;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Education.Delete;

internal sealed class DeleteEducationCommandHandler(
   IApplicationDbContext context,
   ILogger<DeleteEducationCommandHandler> logger) : ICommandHandler<DeleteEducationCommand>
{
    public async Task<Result> Handle(DeleteEducationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling DeleteEducationCommand for EducationId: {EducationId}", command.EducationId);
        
        var education = await context.Educations
            .Where(e => e.Id == command.EducationId && e.UserId == command.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (education == null)
        {
            logger.LogWarning("Education with ID: {EducationId} not found for user {UserId}", command.EducationId, command.UserId);
            return Result.Failure(EducationErrors.EducationNotFound);
        }

        context.Educations.Remove(education);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully deleted education with ID: {EducationId}", education.Id);
        return Result.Success(); 
    }
}