using Application.Abstractions.Messaging;

namespace Application.Users.Education.Get;
public sealed record GetEducationQuery(string UserSlug) : IQuery<List<GetEducationResponse>>;

