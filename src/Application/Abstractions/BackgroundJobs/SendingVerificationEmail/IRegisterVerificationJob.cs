using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;

public interface IRegisterVerificationJob
{
    Task Send(string userEmail, string verificationLink);
    Task SendVerificationEmailAsync(string userEmail,
        string verificationLink,
         PerformContext? context);


}
