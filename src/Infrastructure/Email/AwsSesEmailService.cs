using Amazon.Runtime.Internal.Util;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Application.Abstractions.Email;
using Application.Options;
using Hangfire.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Infrastructure.Email;

internal sealed class AwsSesEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<AwsSesEmailService> _logger;


    public AwsSesEmailService(IAmazonSimpleEmailService sesClient,
                              ILogger<AwsSesEmailService> logger,
                              IOptions<EmailOptions> emailOptions)
    {
        _sesClient = sesClient;
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string recipient,
                                     string subject,
                                     string body,
                                     CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipient))
        {
            throw new ArgumentException("Recipient email cannot be null or empty.", nameof(recipient));
        }
        _logger.LogInformation("Sending email to {Recipient} with subject: {Subject}", recipient, subject);
        var request = new SendEmailRequest
        {
            Source = _emailOptions.SenderEmail,
            Destination = new Destination { ToAddresses = new List<string> { recipient } },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body { Html = new Content(body) }
            }
        };
        try
        {
            // TODO : verify this is working correctly 
            await _sesClient.SendEmailAsync(request, cancellationToken);
        }
        catch (AmazonSimpleEmailServiceException sesEx)
        {
            _logger.LogError(sesEx, "Hangfire Job: AWS SES exception occurred while sending verification email to {Email}", recipient);
            throw;
        }

    }
}