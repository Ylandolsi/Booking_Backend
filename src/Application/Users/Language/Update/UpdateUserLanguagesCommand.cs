using Application.Abstractions.Messaging;

namespace Application.Users.Languages.Update;

public sealed record UpdateUserLanguagesCommand(Guid UserId, List<int>? LanguageIds) : ICommand;