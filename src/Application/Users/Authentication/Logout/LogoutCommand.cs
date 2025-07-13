using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Users.Authentication.Logout;


public sealed record LogoutCommand(int UserId) : ICommand<bool>;

