using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Application.Users.Authentication.Verification;

internal sealed class EmailVerificationSender 
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailVerificationLinkFactory _emailVerificationLinkFactory;
    private readonly IVerificationEmailForRegistrationJob _verificationJob;
    private readonly ILogger<EmailVerificationSender> _logger;

    public EmailVerificationSender(
        UserManager<User> userManager,
        IEmailVerificationLinkFactory emailVerificationLinkFactory,
        IVerificationEmailForRegistrationJob verificationJob,
        ILogger<EmailVerificationSender> logger)
    {
        _userManager = userManager;
        _emailVerificationLinkFactory = emailVerificationLinkFactory;
        _verificationJob = verificationJob;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(User user)
    {
        string emailVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string verificationEmailLink = _emailVerificationLinkFactory.Create(emailVerificationToken, user.Email!);

        _logger.LogInformation("Enqueuing verification email job for {Email}", user.Email);

        try
        {
            await _verificationJob.Send(user.Email!, verificationEmailLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue verification email to {UserEmail}", user.Email);
            throw;
        }
    }
}

