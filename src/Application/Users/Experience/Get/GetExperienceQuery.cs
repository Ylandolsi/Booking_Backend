using Application.Abstractions.Messaging;
using System.Linq.Expressions;


namespace Application.Users.Experience.Get;

public sealed record GetExperienceQuery(Guid UserId) : IQuery<List<GetExperienceResponse>>;

internal sealed class ExperienceResponseMapper
{
    public static Expression<Func<Domain.Users.Entities.Experience, GetExperienceResponse>> Projection
    {
        get
        {
            return experience => new GetExperienceResponse(
                experience.Id,
                experience.Title,
                experience.CompanyName,
                experience.StartDate,
                experience.EndDate,
                experience.Description,
                experience.IsCurrent
            );
        }
    }
}
