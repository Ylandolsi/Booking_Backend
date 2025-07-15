using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Expertise.Update;

internal sealed class UpdateUserExpertiseCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserExpertiseCommandHandler> logger) : ICommandHandler<UpdateUserExpertiseCommand>
{
    public async Task<Result> Handle(UpdateUserExpertiseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating expertise for user {UserId}", command.UserId);

        try
        {
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", command.UserId);
                return Result.Failure(UserErrors.NotFoundById(command.UserId));
            }

            var existingExpertise = await context.UserExpertises
                .Where(ue => ue.UserId == command.UserId)
                .ToListAsync(cancellationToken);

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {


                if (existingExpertise.Any())
                {
                    context.UserExpertises.RemoveRange(existingExpertise);
                }

                // Validate and add new expertise
                if (command.ExpertiseIds?.Count > 0)
                {
                    var validExpertiseIds = await context.Expertises
                        .AsNoTracking()
                        .Where(e => command.ExpertiseIds.Contains(e.Id))
                        .Select(e => e.Id)
                        .ToListAsync(cancellationToken);

                    if (validExpertiseIds?.Count > UserConstraints.MaxExpertises)
                    {
                        return Result.Failure(UserErrors.ExpertiseLimitExceeded);
                    }

                    if (validExpertiseIds?.Count != command.ExpertiseIds.Count)
                    {
                        logger.LogWarning("Some expertise IDs are invalid for user {UserId}", command.UserId);
                        return Result.Failure(Error.Problem("Expertise.InvalidIds", "Some expertise IDs are invalid"));
                    }

                    var userExpertises = command.ExpertiseIds.Select(expertiseId => new UserExpertise
                    {
                        UserId = command.UserId,
                        ExpertiseId = expertiseId
                    }).ToList();

                    await context.UserExpertises.AddRangeAsync(userExpertises, cancellationToken);
                }

                user.ProfileCompletionStatus.UpdateCompletionStatus(user);


                await context.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                logger.LogInformation("Successfully updated {Count} expertise for user {UserId}",
                      command.ExpertiseIds?.Count ?? 0,
                      command.UserId);

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "Failed to update expertise for user {UserId}", command.UserId);
                return Result.Failure(Error.Problem("Expertise.UpdateFailed", "Failed to update expertise"));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update expertise for user {UserId}", command.UserId);
            return Result.Failure(Error.Problem("Expertise.UpdateFailed", "Failed to update expertise"));
        }
    }
}