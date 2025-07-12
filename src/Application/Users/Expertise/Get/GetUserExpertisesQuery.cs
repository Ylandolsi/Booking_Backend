using Application.Abstractions.Messaging;
namespace Application.Users.Expertise.Get;

public sealed record GetUserExpertisesQuery(Guid UserId) : IQuery<List<Domain.Users.Entities.Expertise>>;
