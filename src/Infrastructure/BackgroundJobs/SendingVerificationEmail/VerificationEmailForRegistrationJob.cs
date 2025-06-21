using Amazon.SimpleEmail;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.Email;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Infrastructure.Email.Templates;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Infrastructure.BackgroundJobs.SendingVerificationEmail;

public class VerificationEmailForRegistrationJob : IVerificationEmailForRegistrationJob
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly ILogger<VerificationEmailForRegistrationJob> _logger;

    public VerificationEmailForRegistrationJob(IEmailService emailService,
                                               ILogger<VerificationEmailForRegistrationJob> logger,
                                               IEmailTemplateProvider emailTemplateProvider)
    {
        _emailService = emailService;
        _logger = logger;
        _emailTemplateProvider = emailTemplateProvider;
    }

    public async Task Send(string userEmail, string verificationLink)
    {
        await SendVerificationEmailAsync(userEmail, verificationLink, null);
    }

    [DisplayName("Send Verification Email to {0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendVerificationEmailAsync(
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
            body = body.Replace("{{verificationLink}}", verificationLink);

            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);

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