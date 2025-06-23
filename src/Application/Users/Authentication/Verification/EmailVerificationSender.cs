using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Domain.Users.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Application.Users.Authentication.Verification;

internal sealed class EmailVerificationSender
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailVerificationLinkFactory _emailVerificationLinkFactory;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<EmailVerificationSender> _logger;


    public EmailVerificationSender(UserManager<User> userManager,
                                   IEmailVerificationLinkFactory emailVerificationLinkFactory,
                                   IBackgroundJobClient backgroundJobClient,
                                   ILogger<EmailVerificationSender> logger )
    {
        _userManager = userManager;
        _emailVerificationLinkFactory = emailVerificationLinkFactory;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task SendVerificationEmailAsync(User user)
    {
        _logger.LogInformation("Generating email verification token for user {UserId}", user.Id);

        string emailVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string verificationEmailLink = _emailVerificationLinkFactory.Create(emailVerificationToken, user.Email!);

        _logger.LogInformation("Enqueuing verification email job for {Email}", user.Email);

        try
        {
            _backgroundJobClient.Enqueue<IVerificationEmailForRegistrationJob>(
                            job => job.SendAsync(user.Email!, verificationEmailLink, null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue verification email to {UserEmail}", user.Email);
            throw;
        }
    }


}

