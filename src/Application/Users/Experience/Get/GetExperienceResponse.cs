using System.Linq.Expressions;

namespace Application.Users.Experience.Get;

public sealed record GetExperienceResponse(
    Guid Id,
    string Title,
    string Company,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description,
    bool IsCurrent
);



