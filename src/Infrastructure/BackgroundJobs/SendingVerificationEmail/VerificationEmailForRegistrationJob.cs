using Amazon.SimpleEmail;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.Email;
using Application.Options;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Infrastructure.Email.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Infrastructure.BackgroundJobs.SendingVerificationEmail;

public class VerificationEmailForRegistrationJob : IVerificationEmailForRegistrationJob
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<VerificationEmailForRegistrationJob> _logger;
    private readonly FrontendApplicationOptions _frontendApplicationOptions;

    public VerificationEmailForRegistrationJob(IEmailService emailService,
                                               IOptions<FrontendApplicationOptions> frontendApplicationOptions,
                                               IEmailTemplateProvider emailTemplateProvider ,
                                               ILogger<VerificationEmailForRegistrationJob> logger)
    {
        _emailService = emailService;
        _emailTemplateProvider = emailTemplateProvider;
        _frontendApplicationOptions = frontendApplicationOptions.Value;
        _logger = logger;
    }

    [DisplayName("Send Verification Email to {0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAsync(
                            string userEmail,
                            string verificationLink,
                            PerformContext? context)
    {
        context?.WriteLine($"Attempting to send verification email to: {userEmail}");
        _logger.LogInformation("Hangfire Job: Attempting to send verification email to {Email}", userEmail);

        // provided by the background job server ( eg hangfire)
        // to shutdown gracefully 
        var cancellationToken = context?.CancellationToken.ShutdownToken ?? CancellationToken.None;


        try
        {

            cancellationToken.ThrowIfCancellationRequested();
            var (subject, body) = await _emailTemplateProvider.GetTemplateAsync(TemplatesNames.VerificationEmailForRegistration, cancellationToken);
            body = body.Replace("{{VERIFICATION_LINK}}", verificationLink)
                        .Replace("{{APP_NAME}}",  _frontendApplicationOptions.AppName)
                        .Replace("{{SUPPORT_LINK}}", _frontendApplicationOptions.SupportLink)
                        .Replace("{{SECURITY_LINK}}", _frontendApplicationOptions.SecurityLink);

            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);
            context?.WriteLine($"Verification email sent successfully to: {userEmail}");
            _logger.LogInformation("Hangfire Job: Verification email sent successfully to {Email}", userEmail);

        }
        catch (OperationCanceledException)
        {
            context?.WriteLine("Verification email job was canceled.");
            _logger.LogWarning("Hangfire Job: Verification email job was canceled during shutdown.");
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Unhandled exception while sending verification email to {userEmail}: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: Unhandled exception occurred while sending verification email to {Email}", userEmail);
            throw;
        }
    }
}