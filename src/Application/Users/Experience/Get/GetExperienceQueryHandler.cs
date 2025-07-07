using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;


namespace Application.Users.Experience.Get;

internal sealed class GetExperienceQueryHandler(
   IApplicationDbContext context,
   ILogger<GetExperienceQueryHandler> logger) : IQueryHandler<GetExperienceQuery, List<GetExperienceResponse>>
{


    public async Task<Result<List<GetExperienceResponse>>> Handle(GetExperienceQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetExperienceQuery for UserId: {UserId}", query.UserId);

        var experiences = await context.Experiences
            .Where(x => x.UserId == query.UserId)
            .Select(x => new GetExperienceResponse(
                x.Id,
                x.Title,
                x.CompanyName,
                x.StartDate,
                x.EndDate,
                x.Description,
                x.ToPresent)
            )
            .ToListAsync(cancellationToken);


        if (experiences == null || !experiences.Any())
        {
            logger.LogWarning("No experiences found for UserId: {UserId}", query.UserId);
            return new List<GetExperienceResponse>();
        }

        return experiences;
    }

}