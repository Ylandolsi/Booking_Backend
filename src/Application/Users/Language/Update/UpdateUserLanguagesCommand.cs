using Application.Abstractions.Messaging;

namespace Application.Users.Languages.Update;

public sealed record UpdateUserLanguagesCommand(int UserId, List<int> LanguageIds) : ICommand;