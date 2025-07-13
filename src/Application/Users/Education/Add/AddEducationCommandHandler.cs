using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Education.Add;

internal sealed class AddEducationCommandHandler(IApplicationDbContext context,
                                                  ILogger<AddEducationCommandHandler> logger) : ICommandHandler<AddEducationCommand, int>
{
    public async Task<Result<int>> Handle(AddEducationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding education for user {UserId}", command.UserId);

        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure<int>(UserErrors.NotFoundById(command.UserId));
        }

        var education = new Domain.Users.Entities.Education(
            command.Field,
            command.Description ?? string.Empty,
            command.University,
            command.UserId,
            command.StartDate,
            command.EndDate
        );

        try
        {
            await context.Educations.AddAsync(education, cancellationToken);
            user.ProfileCompletionStatus.UpdateCompletionStatus(user);
            
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add education for user {UserId}", command.UserId);
            return Result.Failure<int>(Error.Problem("Education.AddFailed", "Failed to add education"));
        }

        logger.LogInformation("Successfully added education {EducationId} for user {UserId}",
            education.Id, command.UserId);
        return Result.Success(education.Id);
    }
}