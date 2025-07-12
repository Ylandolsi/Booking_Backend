using Application.Abstractions.Messaging;

namespace Application.Users.Education.Get;
public sealed record GetEducationQuery(Guid UserId) : IQuery<List<GetEducationResponse>>;

