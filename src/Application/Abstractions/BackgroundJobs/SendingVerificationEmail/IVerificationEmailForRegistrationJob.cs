using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;

public interface IVerificationEmailForRegistrationJob
{
    Task SendAsync(string userEmail,
                   string verificationLink,
                   PerformContext? context);


}
