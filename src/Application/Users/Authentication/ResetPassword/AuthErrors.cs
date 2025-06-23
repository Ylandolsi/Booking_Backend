using SharedKernel;

namespace Application.Users.Authentication.ResetPassword;

public static class ResetPasswordErrors
{
    public static readonly Error GenericError =
        Error.Problem("ResetPassword.Failed", "The provided token or email is invalid");
}
