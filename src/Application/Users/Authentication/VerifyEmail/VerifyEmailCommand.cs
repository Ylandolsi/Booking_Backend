using Application.Abstractions.Messaging;

namespace Application.Users.VerifyEmail;


public record VerifyEmailCommand(Guid Token) : ICommand<bool>; 