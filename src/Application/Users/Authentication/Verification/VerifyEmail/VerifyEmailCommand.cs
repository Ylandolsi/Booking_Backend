using Application.Abstractions.Messaging;

namespace Application.Users.Authentication.Verification.VerifyEmail;


public record VerifyEmailCommand(Guid Token) : ICommand<string>; 