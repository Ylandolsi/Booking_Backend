using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;


namespace Application.Users.Education.Get;

internal sealed class GetEducationQueryHandler(
   IApplicationDbContext context,
   ILogger<GetEducationQueryHandler> logger) : IQueryHandler<GetEducationQuery, List<GetEducationResponse>>
{


    public async Task<Result<List<GetEducationResponse>>> Handle(GetEducationQuery query, CancellationToken cancellationToken )
    {
        logger.LogInformation("Handling GetEducationQuery for UserId: {UserId}", query.UserId);

        var educations = await context.Educations
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
            .Select(x => new GetEducationResponse(
                x.Id,
                x.Field,
                x.University,
                x.StartDate,
                x.EndDate,
                x.Description,
                x.ToPresent)
            )
            .ToListAsync(cancellationToken);



        if (educations == null || !educations.Any())
        {
            logger.LogInformation("No educations found for UserId: {UserId}", query.UserId);
            return new List<GetEducationResponse>();
        }

        return educations;
    }

}