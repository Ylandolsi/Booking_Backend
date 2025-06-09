using System.ComponentModel;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using FluentEmail.Core;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs.SendingVerificationEmail;


public class RegisterRegisterVerificationJob : IRegisterVerificationJob
{
    private readonly ILogger<RegisterRegisterVerificationJob> _logger;
    private readonly IFluentEmail _fluentEmail;


    public RegisterRegisterVerificationJob(ILogger<RegisterRegisterVerificationJob> logger, IFluentEmail fluentEmail)
    {
        _logger = logger;
        _fluentEmail = fluentEmail;
    }

    [DisplayName("Send Verification Email to {0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendVerificationEmailAsync(
                            string userEmail,
                            string verificationLink,
                            PerformContext? context) // hanfgire will automatically provide the context 
                                                    // at moment a worker picks a job 
    {

        context?.WriteLine($"Attempting to send verification email to: {userEmail}");
        _logger.LogInformation("Hangfire Job: Attempting to send verification email to {Email}", userEmail);

        try
        {
            var email = _fluentEmail
                .To(userEmail)
                .Subject("Verify your email address - Hangfire")
                .Body($"To verify your email address <a href='{verificationLink}'>click here</a> " +
                      $"or copy and paste the following link into your browser: {verificationLink}", isHtml: true);

            var response = await email.SendAsync();

            if (response.Successful)
            {
                context?.WriteLine($"Verification email successfully sent to: {userEmail}");
                _logger.LogInformation("Hangfire Job: Verification email successfully sent to {Email}", userEmail);
            }
            else
            {
                var errorMessages = string.Join(", ", response.ErrorMessages);
                context?.SetTextColor(ConsoleTextColor.Red);
                context?.WriteLine($"Failed to send verification email to {userEmail}. Errors: {errorMessages}");
                _logger.LogError("Hangfire Job: Failed to send verification email to {Email}. Errors: {Errors}",
                    userEmail, errorMessages);
                // This will cause Hangfire to retry the job based on its default policy
                throw new InvalidOperationException($"Failed to send email: {errorMessages}");
            }
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Unhandled exception while sending verification email to {userEmail}: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: Unhandled exception occurred while sending verification email to {Email}", userEmail);
            throw;
            // Re-throw to allow Hangfire to handle retries ( marking job as failed)
            // by default : hangfire will retry the job 10 times 
            // and each time it will increade the delay between retries
        }
    }
}