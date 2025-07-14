
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

    public const string ForgotPassword = "/users/forgot-password";
    public const string ResetPassword = "/users/reset-password";

    // User Management
    public const string GetUser = "users/{userSlug}"; // TODO : implement this 
    public const string GetCurrentUser = "users/me";

    // Experience 
    public const string GetUserExperiences = "users/experiences{userSlug}";
    public const string AddExperience = "users/experiences";
    public const string UpdateExperience = "users/experiences/{experienceId}";
    public const string DeleteExperience = "users/experiences/{experienceId}";

    // Education 
    public const string GetUserEducations = "users/educations/{userSlug}";
    public const string AddEducation = "users/educations";
    public const string UpdateEducation = "users/education/{educationId}";
    public const string DeleteEducation = "users/education/{educationId}";

    // Expertise
    public const string GetUserExpertises = "users/expertises/{userSlug}";
    public const string GetAllExpertises = "expertises";
    public const string UpdateUserExpertise = "users/expertises";

    // Language
    public const string GetUserLanguages = "users/languages/{userSlug}";
    public const string GetAllLanguages = "languages";
    public const string UpdateUserLanguages = "users/languages";

    // Profile
    public const string UpdateBasicInfo = "users/profile/basic-info";
    public const string UpdateProfilePicture = "users/profile/picture";
    public const string UpdateSocialLinks = "users/profile/social-links";
}