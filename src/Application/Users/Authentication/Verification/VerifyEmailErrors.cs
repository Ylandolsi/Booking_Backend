using SharedKernel;

namespace Application.Users.Authentication.Verification;

public static class VerifyEmailErrors
{

    public static Error EmailVerificationFailed(string message) => Error.Failure(
        "Users.EmailVerificationFailed",
        $"Email verification failed: {message}");

    public static readonly Error TokenExpired = Error.Problem("User.Email.Verification.Token.Expired",
                                                        "Email verification token has expired.");

    public static readonly Error TokenNotFound = Error.NotFound("User.Email.Verification.Token.NotFound",
        "Email verification token not found.");

    public static readonly Error AlreadyVerified = Error.Problem("User.Email.Already.Verified",
        "Email address is already verified.");


    public static Error SendingEmailFailed => Error.Failure("Email.Send.Failed", "Failed to enqueue verification email.");

    public static Error FailedToSaveToken => Error.Failure("Email.VerificationToken.Save.Failed", "Failed to save email verification token.");


    public static Error EmailOrTokenInvalid = Error.Problem("Email.Token.Invalid", "Email or token is invalid");




}

