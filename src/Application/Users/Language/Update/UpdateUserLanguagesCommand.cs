using Application.Abstractions.Messaging;

namespace Application.Users.Languages.Update;

public sealed record UpdateUserLanguagesCommand(Guid UserId, List<Guid> LanguageIds) : ICommand;