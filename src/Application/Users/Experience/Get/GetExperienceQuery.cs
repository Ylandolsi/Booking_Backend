using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Get;
public sealed record GetExperienceQuery(Guid UserId) : IQuery<List<GetExperienceResponse>>;

