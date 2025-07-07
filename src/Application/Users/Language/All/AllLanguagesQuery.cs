using Application.Abstractions.Messaging;

namespace Application.Users.Language.Expose;

public record AllLanguagesQuery : IQuery<List<Domain.Users.Entities.Language>>;
