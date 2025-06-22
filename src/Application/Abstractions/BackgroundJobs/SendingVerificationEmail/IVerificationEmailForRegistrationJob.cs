using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;

public interface IVerificationEmailForRegistrationJob
{
    Task SendVerificationEmailAsync(string userEmail,
        string verificationLink,
         PerformContext? context);


}
