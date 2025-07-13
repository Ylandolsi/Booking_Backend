using Application.Abstractions.Messaging;
namespace Application.Users.Expertise.Get;

public sealed record GetUserExpertisesQuery(string UserSlug) : IQuery<List<Domain.Users.Entities.Expertise>>;
