using Application.Abstractions.Messaging;

namespace Application.Users.Authentication.ResetPassword.Send;

public record RestPasswordCommand(string Email) : ICommand;
