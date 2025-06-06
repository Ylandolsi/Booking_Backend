using System.Data;
using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static readonly Error PasswordCannotBeEmpty = Error.Failure(
        "Users.PasswordCannotBeEmpty",
        "The password cannot be empty. Please provide a valid password.");

    public static Error InvalideX(string x)
        => Error.Failure(
            $"Users.Invalide{x}",
            $"The provided value for {x} is invalid. Please provide a valid {x}.");
    
}
