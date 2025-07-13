using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using ExpertiseE = Domain.Users.Entities.Expertise;
namespace Application.Users.Expertise.Get;

internal sealed class GetUserExpertisesQueryHandler(
    IApplicationDbContext context,
    ILogger<GetUserExpertisesQueryHandler> logger) : IQueryHandler<GetUserExpertisesQuery, List<ExpertiseE>>
{

    public async Task<Result<List<ExpertiseE>>> Handle(GetUserExpertisesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetUserExpertisesQuery for UserSlug: {UserSlug}", query.UserSlug);
        // TODO : add cache here 
        int? userId = await context.Users.Where(u => u.Slug == query.UserSlug).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);

        if (userId == null)
        {
            logger.LogWarning("User not found for UserId: {UserId}", query.UserSlug);
            return Result.Failure<List<ExpertiseE>>(UserErrors.NotFoundBySlug(query.UserSlug)); 
        }

        var userExpertises = await context.UserExpertises
            .AsNoTracking()
            .Where(ul => ul.UserId == userId)
            .Select(ul => ul.Expertise)
            .ToListAsync(cancellationToken);

        return userExpertises;
    }
}