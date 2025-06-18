using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Options;
using FluentEmail.Core;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Infrastructure.BackgroundJobs.SendingVerificationEmail
{
    public class VerificationEmailForRegistrationJob : IRegisterVerificationJob
    {
        private readonly ILogger<VerificationEmailForRegistrationJob> _logger;
        private readonly IAmazonSimpleEmailService _emailService;
        private readonly EmailOptions _emailOptions;

        public VerificationEmailForRegistrationJob(IAmazonSimpleEmailService emailService,
                                                   IOptions<EmailOptions> emailOptions,
                                                   ILogger<VerificationEmailForRegistrationJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
            _emailOptions = emailOptions.Value ?? throw new ArgumentNullException(nameof(emailOptions), "Email options cannot be null");

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

            try
            {
                var request = new SendEmailRequest
                {
                    Source = _emailOptions.SenderEmail,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { userEmail }
                    },
                    Message = new Message
                    {
                        Subject = new Content("Verify your email address"),
                        Body = new Body
                        {
                            Html = new Content($"To verify your email address <a href='{verificationLink}'>click here</a> " +
                                               $"or copy and paste the following link into your browser: {verificationLink}"),
                            //Text = new Content($"To verify your email address, visit: {verificationLink}")
                        }
                    }
                };

                var response = await _emailService.SendEmailAsync(request);

                // if (response.ResponseMetadata.ChecksumValidationStatus == Amazon.Runtime.ChecksumValidationStatus.SUCCESSFUL)
                // {
                //     context?.WriteLine($"Verification email successfully sent to: {userEmail}");
                //     _logger.LogInformation("Hangfire Job: Verification email successfully sent to {Email}", userEmail);
                // }
                // else
                // {
                //     _logger.LogWarning("Hangfire Job: SES response not validated for {Email}. Status: {Status}, MessageId: {MessageId}",
                //         userEmail, response.ResponseMetadata.ChecksumValidationStatus, response.MessageId);

                //     throw new InvalidOperationException($"Failed to send email to {userEmail}. SES response status: {response.ResponseMetadata.ChecksumValidationStatus}");
                // }
            }
            catch (AmazonSimpleEmailServiceException sesEx)
            {
                context?.SetTextColor(ConsoleTextColor.Red);
                context?.WriteLine($"AWS SES exception while sending verification email to {userEmail}: {sesEx.Message}");
                _logger.LogError(sesEx, "Hangfire Job: AWS SES exception occurred while sending verification email to {Email}", userEmail);
                throw;
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
}