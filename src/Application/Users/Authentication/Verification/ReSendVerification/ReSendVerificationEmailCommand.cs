using Application.Abstractions.Messaging;

namespace Application.Users.ReSendVerification;

public sealed record ReSendVerificationEmailCommand(string Email)
    : ICommand;
