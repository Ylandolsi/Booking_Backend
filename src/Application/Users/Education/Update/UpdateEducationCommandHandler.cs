using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Education.Update;

internal sealed class UpdateEducationCommandHandler(
    IApplicationDbContext context,
    ILogger<UpdateEducationCommandHandler> logger) : ICommandHandler<UpdateEducationCommand>
{
    public async Task<Result> Handle(UpdateEducationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating educaiton {EducationId} for user {UserId}", command.EducationId, command.UserId);


        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure(UserErrors.NotFoundById(command.UserId));
        }

        var educaiton = await context.Educations
            .Where(e => e.Id == command.EducationId && e.UserId == command.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (educaiton == null)
        {
            logger.LogWarning("Education with ID {EducationId} not found for user {UserId}", command.EducationId, command.UserId);
            return Result.Failure(EducationErrors.EducationNotFound);
        }

        try
        {
            educaiton.Update(
                command.Field,
                command.University,
                command.StartDate,
                command.EndDate,
                command.Description
            );

            user.ProfileCompletionStatus.UpdateCompletionStatus(user);

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update educaiton {EducationId} for user {UserId}", command.EducationId, command.UserId);
            return Result.Failure<int>(Error.Problem("Education.UpdateFailed", "Failed to update educaiton"));
        }

        logger.LogInformation("Successfully updated educaiton {EducationId} for user {UserId}", command.EducationId, command.UserId);
        return Result.Success();
    }
}