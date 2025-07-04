//using Application.Abstractions.Data;
//using Application.Abstractions.Messaging;
//using Application.Users.Register;
//using Domain.Users.Entities;
//using Microsoft.Extensions.Logging;
//using SharedKernel;

//namespace Application.Users.Experience.Add;

//internal sealed class AddExperienceCommandHandler(
//    IApplicationDbContext context,
//    ILogger<AddExperienceCommandHandler> logger) : ICommandHandler<AddExperienceCommand, Guid>
//{
//    public async Task<Result<Guid>> Handle(AddExperienceCommand command, CancellationToken cancellationToken = default)
//    {
//        logger.LogInformation("Handling AddExperienceCommand for UserId: {UserId}", command.UserId);
//        var ExperienceRes = Domain.Users.Entities.Experience.Create(command.Title,
//                                                  command.UserId,
//                                                  command.Company,
//                                                  command.StartDate,
//                                                  command.EndDate,
//                                                  command.Description);
//        if (ExperienceRes.IsFailure)
//        {
//            logger.LogWarning("Failed to create experience for UserId: {UserId}. Error: {Error}", command.UserId, ExperienceRes.Error);
//            return Result.Failure<Guid>(ExperienceRes.Error);
//        }

//        await context.Experiences.AddAsync(ExperienceRes.Value);
//        await context.SaveChangesAsync(cancellationToaken);
//        logger.LogInformation("Successfully added experience for UserId: {UserId}, ExperienceId: {ExperienceId}", command.UserId, ExperienceRes.Value.Id);
//        return ExperienceRes.Value.Id;

//    }

//}
