using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;

namespace Application.Users.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;

