using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Language.Expose;

internal sealed class AllLanguagesQuryHandler(IApplicationDbContext applicationDbContext,
                                              ILogger<AllLanguagesQuryHandler> logger) : IQueryHandler<AllLanguagesQuery, List<Domain.Users.Entities.Language>>
{
    public async Task<Result<List<Domain.Users.Entities.Language>>> Handle(AllLanguagesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling AllLanguagesQuery");

        var languages = await applicationDbContext.Languages
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (languages == null || !languages.Any())
        {
            logger.LogWarning("No languages found in the database");
            return new List<Domain.Users.Entities.Language>();
        }
        return languages;

    }
}
