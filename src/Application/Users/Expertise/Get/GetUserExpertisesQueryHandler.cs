using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
namespace Application.Users.Expertise.Get;

internal sealed class GetUserExpertisesQueryHandler(
    IApplicationDbContext context,
    ILogger<GetUserExpertisesQueryHandler> logger) : IQueryHandler<GetUserExpertisesQuery, List<Domain.Users.Entities.Expertise>>
{

    public async Task<Result<List<Domain.Users.Entities.Expertise>>> Handle(GetUserExpertisesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetUserExpertisesQuery for UserId: {UserId}", query.UserId);

        var userExpertises = await context.UserExpertises
            .AsNoTracking()
            .Where(ul => ul.UserId == query.UserId)
            .Select(ul => ul.Expertise)
            .ToListAsync(cancellationToken);

        return userExpertises;
    }
}