using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Domain.Users.Entities; 

namespace Application.Users.ReSendVerification;

internal sealed class ReSendVerificationEmailCommandHandler(
    IApplicationDbContext context,
    ILogger<ReSendVerificationEmailCommandHandler> logger,
    IEmailVerificationLinkFactory emailVerificationLinkFactory,
    IRegisterVerificationJob registerVerificationJob) 
    : ICommandHandler<ReSendVerificationEmailCommand, bool>{

    public async Task<Result<bool>> Handle(ReSendVerificationEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling ReSendVerificationEmailCommand for user ID: {UserId}", command.UserId);


        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", command.UserId);
            return Result.Failure<bool>(UserErrors.NotFound(command.UserId));
        }

        logger.LogInformation("User with ID {UserId} found, checking email verification status for {Email}", command.UserId, user.EmailAddress.Email);

        if (user.EmailAddress.Verified)
        {
            logger.LogInformation("User with email {Email} already verified", user.EmailAddress.Email);
            return Result.Failure<bool>(Error.Problem("Email.AlreadyVerified", "The email address is already verified."));
        }

        EmailVerificationToken? emailVerificationToken = await context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id, cancellationToken);

        if (emailVerificationToken != null && !emailVerificationToken.IsStillValid())
        {
            logger.LogInformation("Existing email verification token found for user ID {UserId} but it has expired. A new one will be created.", user.Id);
            context.EmailVerificationTokens.Remove(emailVerificationToken);
            emailVerificationToken = null;
        }

        if (emailVerificationToken == null)
        {
            logger.LogInformation("No valid email verification token found for user ID {UserId}. Creating a new one.", user.Id);
            emailVerificationToken = new EmailVerificationToken(user.Id);
            context.EmailVerificationTokens.Add(emailVerificationToken);
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation("New email verification token created and saved for user ID {UserId}", user.Id);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Failed to save new email verification token for user ID: {UserId}", user.Id);
                return Result.Failure<bool>(UserErrors.UserRegistrationFailed("Failed to save new verification token."));
            }
        }
        else
        {
            logger.LogInformation("Valid email verification token found for user ID {UserId}", user.Id);

            emailVerificationToken.UpdateExpiration();
            logger.LogInformation("Email verification token for user ID {UserId} updated with new expiration time", user.Id);
        }

        logger.LogInformation("Preparing to send verification email to {Email}", user.EmailAddress.Email);

        string verificationEmailLink = emailVerificationLinkFactory.Create(emailVerificationToken);
        logger.LogInformation("Verification email link created for email {Email}: {Link}", user.EmailAddress.Email, verificationEmailLink);

        try
        {
            await registerVerificationJob.Send(user.EmailAddress.Email, verificationEmailLink);
            logger.LogInformation("Verification email re-send job enqueued for {Email}", user.EmailAddress.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to enqueue re-send verification email to {UserEmail}", user.EmailAddress.Email);
            
            return Result.Failure<bool>(Error.Failure("Email.Send.Failed", "Failed to enqueue verification email."));
        }

        return Result.Success(true);
    }
}