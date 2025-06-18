using System.Data;
using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFoundById(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error NotFoundByEmail(string email) => Error.NotFound(
        "Users.NotFoundByEmail",
        $"The user with the email = '{email}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error EmailIsNotVerified = Error.Problem(
        "Users.EmailIsNotVerified",
        "The email address is not verified. Please verify your email before proceeding.");

    public static readonly Error IncorrectEmailOrPassword = Error.Problem(
    "Users.IncorrectEmailOrPassword",
    "The provided email or password is incorrect. Please try again.");

    public static readonly Error AccountLockedOut = Error.Unauthorized(
        "Users.AccountLockedOut",
        "This account has been locked out due to too many failed login attempts. Please try again later.");


    public static readonly Error TokenGenerationFailed = Error.Failure(
        "Users.TokenGenerationFailed",
        "Failed to generate a token for the user. Please try again later.");

    public static readonly Error PasswordCannotBeEmpty = Error.Problem(
        "Users.PasswordCannotBeEmpty",
        "The password cannot be empty. Please provide a valid password.");




    public static Error InvalideX(string x)
        => Error.Problem(
            $"Users.Invalide{x}",
            $"The provided value for {x} is invalid. Please provide a valid {x}.");

}
