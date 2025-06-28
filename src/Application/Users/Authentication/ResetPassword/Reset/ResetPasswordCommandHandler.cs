using Application.Abstractions.BackgroundJobs.SendingPasswordResetToken;
using Application.Abstractions.Messaging;
using Application.Options;
using Domain.Users.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;
using System.Web;

namespace Application.Users.Authentication.ResetPassword.Send;

internal sealed class ResetPasswordCommandHandler(UserManager<User> userManager,
                                                 IOptions<FrontendApplicationOptions> frontendApplicationOptions,
                                                 IBackgroundJobClient backgroundJobClient,
                                                 ILogger<ResetPasswordCommandHandler> logger) : ICommandHandler<RestPasswordCommand>
{

    private readonly FrontendApplicationOptions frontendApplicationOptions = frontendApplicationOptions.Value;
    public async Task<Result> Handle(RestPasswordCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sending password reset token to {Email}", command.Email);

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            await SimulatePasswordResetWorkAsync(); 
        }
        else
        {

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var builder = new UriBuilder(frontendApplicationOptions.BaseUrl)
            {
                Path = frontendApplicationOptions.PasswordReset,
                Query = $"token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(command.Email)}"
            };
            var resetUrl = builder.ToString();

            backgroundJobClient.Enqueue<ISendingPasswordResetToken>(
                    job => job.SendAsync(command.Email, resetUrl, null));

        }
        return Result.Success();
        // TODO : front end display 
        // If an account exists with this email, you'll receive a reset link"
    }

    private static async Task SimulatePasswordResetWorkAsync()
    {
        var delay = Random.Shared.Next(150, 250);
        await Task.Delay(delay);
    }
}

