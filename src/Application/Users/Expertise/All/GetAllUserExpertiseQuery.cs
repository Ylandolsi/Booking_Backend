using Application.Abstractions.Messaging;

namespace Application.Users.Expertise.Get;

public sealed record GetAllExpertiseQuery() : IQuery<List<Domain.Users.Entities.Expertise>>;
