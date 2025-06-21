using Application.Abstractions.Messaging;

namespace Application.Users.ReSendVerification;

public sealed record ReSendVerificationCommand(string Email)
    : ICommand;
