
public static class UsersEndpoints
{
    public const string GoogleLogin = "users/login/google";
    public const string GoogleLoginCallback = "users/login/google/callback";
    // Authentication and Authorization
    public const string Login = "users/login";
    public const string Register = "users/register";
    public const string RefreshAccessToken = "users/refresh-token";
    public const string VerifyEmail = "users/verify-email";
    public const string ResendVerificationEmail = "users/resend-verification-email";
    public const string Logout = "users/logout";
    public const string ChangePassword = "users/change-password";

    public const string ResetPassword = "users/reset-password-email";
    public const string ResetPasswordSendToken = "/users/reset-password/send-token";
    public const string ResetPasswordVerify = "/users/reset-password/verify";

    // User Management
    public const string GetUser = "users/{id}";

    // Experience 

    // Education 

}