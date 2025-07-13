using SharedKernel;

namespace Application.Users.Register;

public static class RegisterErrors
{
    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static Error UserRegistrationFailed(string message) => Error.Failure(
        "Users.UserRegistrationFailed",
        message);
}