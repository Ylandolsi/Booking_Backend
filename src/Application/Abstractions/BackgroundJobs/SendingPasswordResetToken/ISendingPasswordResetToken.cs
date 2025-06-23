
using Hangfire.Server;

namespace Application.Abstractions.BackgroundJobs.SendingPasswordResetToken;

public interface ISendingPasswordResetToken
{
    Task SendAsync(string userEmail,
                   string resetUrl,
                   PerformContext? context );
}
