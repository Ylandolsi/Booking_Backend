using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Get;
public sealed record GetExperienceQuery(string UserSlug) : IQuery<List<GetExperienceResponse>>;

