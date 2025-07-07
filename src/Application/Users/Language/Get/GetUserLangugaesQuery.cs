using Application.Abstractions.Messaging;
using Domain.Users.Entities;
namespace Application.Users.Language.Get;

public record GetUserLanguagesQuery(Guid UserId) : IQuery<List<Domain.Users.Entities.Language>>;
