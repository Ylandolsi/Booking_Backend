using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
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
        logger.LogInformation("Handling GetEducationQuery for UserSlug: {UserSlug}", query.UserSlug);
        // TODO : add cache here 
        int? userId = await context.Users.Where(u => u.Slug == query.UserSlug).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);

        if (userId == null)
        {
            logger.LogWarning("User not found for UserId: {UserId}", query.UserSlug);
            return Result.Failure<List<GetEducationResponse>>(UserErrors.NotFoundBySlug(query.UserSlug)); 
        }
        var educations = await context.Educations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
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
            logger.LogInformation("No educations found for UserId: {UserId}", userId);
            return new List<GetEducationResponse>();
        }

        return educations;
    }

}