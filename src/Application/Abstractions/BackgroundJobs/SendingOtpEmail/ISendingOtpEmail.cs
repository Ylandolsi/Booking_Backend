using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingOtpEmail;

public interface ISendingOtpEmail
{
    Task SendAsync(string email, string otp , PerformContext? context);
}