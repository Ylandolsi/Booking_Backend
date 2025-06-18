using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Verification;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.ReSendVerification;





internal sealed class ReSendVerificationEmailCommandHandler(UserManager<User> userManager,
                                                            ILogger<ReSendVerificationEmailCommandHandler> logger,
                                                            IEmailVerificationLinkFactory emailVerificationLinkFactory,
                                                            IRegisterVerificationJob registerVerificationJob)
    : ICommandHandler<ReSendVerificationEmailCommand>
{

    public async Task<Result> Handle(ReSendVerificationEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling ReSendVerificationEmailCommand for user Email: {Email}", command.Email);


        User? user = await userManager.FindByEmailAsync(command.Email);

        if (user == null)
        {
            logger.LogWarning("User with Email {Email} not found", command.Email);
            return Result.Failure(UserErrors.NotFoundByEmail(command.Email));
        }

        logger.LogInformation("Checking email verification status for {Email}", command.Email);

        if (user.EmailConfirmed)
        {
            logger.LogInformation("User with Email {Email} already has a verified email address", command.Email);
            return Result.Failure(VerifyEmailErrors.AlreadyVerified);
        }

        string emailVerificationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        string verificationEmailLink = emailVerificationLinkFactory.Create(emailVerificationToken, command.Email);
        logger.LogInformation("Sending verification email to {Email}", command.Email);

        try
        {
            // background job
            await registerVerificationJob.Send(
                command.Email,
                verificationEmailLink
            );

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to enqueue verification email to {UserEmail}", command.Email);
            return Result.Failure(VerifyEmailErrors.SendingEmailFailed);

        }

        return Result.Success();
    }
}