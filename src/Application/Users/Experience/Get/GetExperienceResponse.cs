using System.Linq.Expressions;

namespace Application.Users.Experience.Get;

public sealed record GetExperienceResponse(
    Guid Id,
    string Title,
    string Company,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description , 
    bool IsCurrent 
);


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
