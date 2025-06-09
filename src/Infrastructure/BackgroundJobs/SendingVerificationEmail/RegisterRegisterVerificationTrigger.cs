using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Hangfire;

namespace Infrastructure.BackgroundJobs.SendingVerificationEmail;

public class RegisterRegisterVerificationTrigger : IRegisterVerificationTrigger
{
    private readonly IBackgroundJobClient BackgroundJobClient;


    public RegisterRegisterVerificationTrigger(IBackgroundJobClient backgroundJobClient)
    {
        BackgroundJobClient = backgroundJobClient;
    }

    public Task Send(string email, string verificationLink)
    {
        BackgroundJobClient.Enqueue<IRegisterVerificationJob>(job =>
            job.SendVerificationEmailAsync(email, verificationLink, null));
        
        return Task.CompletedTask;

    }
}