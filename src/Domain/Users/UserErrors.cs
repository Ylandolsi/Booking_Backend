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

    public static readonly Error IncorrectPassword = Error.Failure(
        "Users.IncorrectPassword",
        "The provided password is incorrect. Please try again.");

    public static readonly Error IncorrectEmailOrPassword = Error.Problem(
        "Users.IncorrectEmailOrPassword",
        "The provided email or password is incorrect. Please try again.");

    public static readonly Error TokenGenerationFailed = Error.Failure(
        "Users.TokenGenerationFailed",
        "Failed to generate a token for the user. Please try again later.");

    public static Error EmailVerificationFailed(string message) => Error.Failure(
        "Users.EmailVerificationFailed",
        $"Email verification failed: {message}");
       
    public static Error UserRegistrationFailed(string message) => Error.Failure(
        "Users.UserRegistrationFailed",
        $"User registration failed: {message}");

    

    public static Error InvalideX(string x)
        => Error.Problem(
            $"Users.Invalide{x}",
            $"The provided value for {x} is invalid. Please provide a valid {x}.");
    
}
