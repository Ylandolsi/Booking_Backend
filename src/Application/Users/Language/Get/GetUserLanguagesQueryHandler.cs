using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
namespace Application.Users.Language.Get;

internal sealed class GetUserLanguagesQueryHandler(
    IApplicationDbContext context,
    ILogger<GetUserLanguagesQuery> logger) : IQueryHandler<GetUserLanguagesQuery, List<Domain.Users.Entities.Language>>
{

    public async Task<Result<List<Domain.Users.Entities.Language>>> Handle(GetUserLanguagesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetUserLanguagesQuery for UserId: {UserId}", query.UserId);

        var userLanguages = await context.UserLanguages
            .AsNoTracking()
            .Where(ul => ul.UserId == query.UserId)
            .Select(ul => ul.Language)
            .ToListAsync(cancellationToken);

        return userLanguages;
    }
}