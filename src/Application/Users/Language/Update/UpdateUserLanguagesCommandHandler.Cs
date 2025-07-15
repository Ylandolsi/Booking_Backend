using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Languages.Update;

internal sealed class UpdateUserLanguagesCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserLanguagesCommandHandler> logger) : ICommandHandler<UpdateUserLanguagesCommand>
{
    public async Task<Result> Handle(UpdateUserLanguagesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating languages for user {UserId}", command.UserId);

        try
        {
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", command.UserId);
                return Result.Failure(UserErrors.NotFoundById(command.UserId));
            }

            // Remove existing user languages
            var existingLanguages = await context.UserLanguages
            .Where(ul => ul.UserId == command.UserId)
            .ToListAsync(cancellationToken);

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {

                if (existingLanguages.Any())
                {
                    context.UserLanguages.RemoveRange(existingLanguages);
                    logger.LogInformation("Deleting existing languages for user {UserId}", command.UserId);
                }
                if (command.LanguageIds.Any())
                {
                    var validLanguageIds = await context.Languages
                        .AsNoTracking()
                        .Where(l => command.LanguageIds.Contains(l.Id))
                        .Select(l => l.Id)
                        .ToListAsync(cancellationToken);


                    if (validLanguageIds?.Count > UserConstraints.MaxExpertises)
                    {
                        return Result.Failure(UserErrors.LanguageLimitExceeded);
                    }


                    if (validLanguageIds?.Count != command.LanguageIds.Count)
                    {
                        logger.LogWarning("Some language IDs are invalid for user {UserId}", command.UserId);
                        return Result.Failure(Error.Problem("Languages.InvalidIds", "Some language IDs are invalid"));
                    }

                    var userLanguages = command.LanguageIds.Select(languageId => new UserLanguage
                    {
                        UserId = command.UserId,
                        LanguageId = languageId
                    }).ToList();



                    await context.UserLanguages.AddRangeAsync(userLanguages, cancellationToken);
                    user.ProfileCompletionStatus.UpdateCompletionStatus(user);
                    await context.SaveChangesAsync(cancellationToken);
                    await unitOfWork.CommitTransactionAsync(cancellationToken);

                    logger.LogInformation("Successfully updated {Count} languages for user {UserId}",
                        command.LanguageIds.Count, command.UserId);

                }

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "Failed to update languages for user {UserId}", command.UserId);
                return Result.Failure(Error.Problem("Languages.UpdateFailed", "Failed to update languages"));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update languages for user {UserId}", command.UserId);
            return Result.Failure(Error.Problem("Languages.UpdateFailed", "Failed to update languages"));
        }
    }
}