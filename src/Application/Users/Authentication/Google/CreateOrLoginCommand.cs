
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using SharedKernel;
using System.Security.Claims;

namespace Application.Users.Authentication.Google;

public record CreateOrLoginCommand(ClaimsPrincipal principal) : ICommand<LoginResponse>;

public static class CreateOrLoginErrors
{
    public static Error UserRegistrationFailed(string message) => Error.Failure("UserRegistrationFailed", message);
}

