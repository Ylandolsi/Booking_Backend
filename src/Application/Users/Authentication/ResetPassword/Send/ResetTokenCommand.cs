using Application.Abstractions.Messaging;

namespace Application.Users.Authentication.ResetPassword.Send;

public record ResetTokenCommand(string Email) : ICommand;
