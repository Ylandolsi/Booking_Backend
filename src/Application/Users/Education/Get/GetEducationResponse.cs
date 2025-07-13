using System.Linq.Expressions;

namespace Application.Users.Education.Get;

public sealed record GetEducationResponse(
    int Id,
    string Field,
    string University,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description,
    bool ToPresent
);



