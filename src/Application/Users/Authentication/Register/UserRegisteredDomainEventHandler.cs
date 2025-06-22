using Application.Users.Authentication.Verification;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Authentication.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{

    private readonly UserManager<User> _userManager;
    private readonly EmailVerificationSender _emailVerificationSender;
    private readonly ILogger<UserRegisteredDomainEventHandler> _logger;

    public UserRegisteredDomainEventHandler(
        UserManager<User> userManager,
        EmailVerificationSender emailVerificationSender,
        ILogger<UserRegisteredDomainEventHandler> logger)
    {
        _userManager = userManager;
        _emailVerificationSender = emailVerificationSender;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UserRegisteredDomainEvent for user ID: {UserId}", notification.UserId);
        User? user = await _userManager.FindByIdAsync(notification.UserId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found, could not send verification email.", notification.UserId);
            return;
        }

        try
        {
            await _emailVerificationSender.SendVerificationEmailAsync(user);
            _logger.LogInformation("Successfully enqueued verification email for user {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue verification email for user {UserId}", user.Id);
            throw; // Re-throwing the exception to ensure that the job is retried :
                   // there is pipeline for this job 
                   // resilience pipline that is used for publishing domain events
                   // and it is used in the ProcessOutboxMessagesJob
                   // so to make sure that the email verification is sent
                   // if something goes wrong, we can retry the job
                   // by throwing exception in the handler, it will be retried by polly 
                   // polly detect exception and retry the job
        }
    }
}