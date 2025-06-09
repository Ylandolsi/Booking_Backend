namespace Application.Abstractions.BackgroundJobs.SendingVerificationEmail;

public interface IRegisterVerificationTrigger
{
    Task Send(string email, string verificationLink);

}