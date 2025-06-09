using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
public interface IRegisterVerificationJob
{
    Task SendVerificationEmailAsync(string userEmail,
        string verificationLink,
         PerformContext? context);
}
