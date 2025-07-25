using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;


namespace Application.Users.Experience.Get;

internal sealed class GetExperienceQueryHandler(
   IApplicationDbContext context,
   ILogger<GetExperienceQueryHandler> logger) : IQueryHandler<GetExperienceQuery, List<GetExperienceResponse>>
{


    public async Task<Result<List<GetExperienceResponse>>> Handle(GetExperienceQuery query, CancellationToken cancellationToken )
    {
        logger.LogInformation("Handling GetExperienceQuery for UserSlug: {UserSlug}", query.UserSlug);
        // get the internal id of user then join 
        // TODO : add cache here 
        int? userId = await context.Users.Where(u => u.Slug == query.UserSlug).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);

        if (userId == null)
        {
            logger.LogWarning("User not found for UserId: {UserId}", query.UserSlug);
            return Result.Failure<List<GetExperienceResponse>>(UserErrors.NotFoundBySlug(query.UserSlug)); 
        }
        
        
        var experiences = await context.Experiences
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new GetExperienceResponse(
                x.Id,
                x.Title,
                x.Company,
                x.StartDate,
                x.EndDate,
                x.Description,
                x.ToPresent)
            )
            .ToListAsync(cancellationToken);



        if (experiences == null || !experiences.Any())
        {
            logger.LogWarning("No experiences found for UserId: {UserId}", userId);
            return new List<GetExperienceResponse>();
        }

        return experiences;
    }

}