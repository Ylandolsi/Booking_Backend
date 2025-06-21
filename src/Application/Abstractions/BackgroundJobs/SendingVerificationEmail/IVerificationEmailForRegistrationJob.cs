using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;

public interface IVerificationEmailForRegistrationJob
{
    Task Send(string userEmail, string verificationLink);
    Task SendVerificationEmailAsync(string userEmail,
        string verificationLink,
         PerformContext? context);


}
