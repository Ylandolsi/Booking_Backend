using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Verification;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Register;


public static class RegisterUserErrors
{
    public static readonly Error EmailNotUnique = Error.Problem(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static Error UserRegistrationFailed(string message) => Error.Problem(
        "Users.UserRegistrationFailed",
        message);
}
internal sealed class RegisterUserCommandHandler(UserManager<User> userManager,
                                                 IEmailVerificationLinkFactory emailVerificationLinkFactory,
                                                 IRegisterVerificationJob registerVerificationJob,
                                                 ILogger<RegisterUserCommandHandler> logger)
    : ICommandHandler<RegisterUserCommand>
{

    public async Task<Result> Handle(RegisterUserCommand command,
                                     CancellationToken cancellationToken)
    {


        if (await userManager.FindByEmailAsync(command.Email) != null)
        {
            logger.LogWarning("Attempt to register user with non-unique email: {Email}", command.Email);
            return Result.Failure(RegisterUserErrors.EmailNotUnique);
        }

        logger.LogInformation("Registering user with email: {Email}", command.Email);


        User user = User.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            command.ProfilePictureSource
        );

        IdentityResult result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to register user with email: {Email}. Errors: {Errors}", command.Email, result.Errors);
            return Result.Failure(RegisterUserErrors.UserRegistrationFailed(string.Join(", ", result.Errors.Select(e => e.Description))));
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
        logger.LogInformation("Verification email job enqueued for {Email}", command.Email);


        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return Result.Success();

        // TODO : in front end : 
        // message to show:  Registration successful! A verification email has been sent.
        //  If you don't receive it within a few minutes, 
        // please check your spam folder or use the 'Resend verification' option on the login page
    }


}
