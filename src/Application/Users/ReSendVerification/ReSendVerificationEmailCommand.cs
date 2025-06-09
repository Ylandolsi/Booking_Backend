using Application.Abstractions.Messaging;

namespace Application.Users.ReSendVerification;

public sealed record ReSendVerificationEmailCommand(Guid UserId )
    : ICommand<bool>;
