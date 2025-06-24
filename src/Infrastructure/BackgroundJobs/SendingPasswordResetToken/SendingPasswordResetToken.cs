using Application.Abstractions.BackgroundJobs.SendingPasswordResetToken;
using Application.Abstractions.Email;
using Application.Options;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Infrastructure.Email.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;


namespace Infrastructure.BackgroundJobs.SendingPasswordResetToken;


public class SendingPasswordResetToken : ISendingPasswordResetToken
{

    private readonly IEmailService _emailService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly FrontendApplicationOptions _frontendApplicationOptions;
    private readonly ILogger<SendingPasswordResetToken> _logger;


    public SendingPasswordResetToken(IEmailService emailService,
                           IEmailTemplateProvider emailTemplateProvider, 
                           IOptions<FrontendApplicationOptions> frontendApplicationOptions,
                           ILogger<SendingPasswordResetToken> logger )
    {
        _emailService = emailService;
        _emailTemplateProvider = emailTemplateProvider;
        _frontendApplicationOptions = frontendApplicationOptions.Value;
        _logger = logger;
    }


    [DisplayName("Send password reset token to {0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAsync(
                            string userEmail,
                            string resetUrl,
                            PerformContext? context  )
    {

        context?.WriteLine($"Attempting to send password reset token to email : {userEmail}");
        _logger.LogInformation("Hangfire Job: Attempting to send password reset token to email : {Email}", userEmail);

        // provided by the background job server ( eg hangfire)
        // to shutdown gracefully 
        var cancellationToken = context?.CancellationToken.ShutdownToken ?? CancellationToken.None;

        try
        {

            cancellationToken.ThrowIfCancellationRequested();
            var (subject, body) = await _emailTemplateProvider.GetTemplateAsync(TemplatesNames.PasswordResetEmail, cancellationToken);
            body = body.Replace("{{RESET_LINK}}", resetUrl)
                        .Replace("{{APP_NAME}}", _frontendApplicationOptions.AppName)
                        .Replace("{{SUPPORT_LINK}}", _frontendApplicationOptions.SupportLink)
                        .Replace("{{SECURITY_LINK}}", _frontendApplicationOptions.SecurityLink); 

            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);
            context?.WriteLine($"token is sent successfully to : {userEmail}");
            _logger.LogInformation("Hangfire Job: token is sent successfully to ", userEmail);

        }
        catch (OperationCanceledException)
        {
            context?.WriteLine("Sending Reset token email job was canceled.");
            _logger.LogWarning("Hangfire Job: Sending Reset token email job was canceled during shutdown.");
        }
        catch (Exception ex)
        {
            context?.SetTextColor(ConsoleTextColor.Red);
            context?.WriteLine($"Unhandled exception while sending Reset token to email {userEmail}: {ex.Message}");
            _logger.LogError(ex, "Hangfire Job: exception while sending Reset token to email {Email}", userEmail);
            throw;
        }

    }
}
