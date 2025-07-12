using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Expertise.Get;

internal sealed class GetAllExpertiseQueryHandler(
    IApplicationDbContext context,
    ILogger<GetAllExpertiseQueryHandler> logger) : IQueryHandler<GetAllExpertiseQuery, List<Domain.Users.Entities.Expertise>>
{
    public async Task<Result<List<Domain.Users.Entities.Expertise>>> Handle(GetAllExpertiseQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAllExpertiseQuery");

        var expertises = await context.Expertises
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success(expertises);
    }
}