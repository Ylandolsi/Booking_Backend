using Application.Abstractions.Authentication;
using Application.Abstractions.BackgroundJobs;
using Application.Abstractions.BackgroundJobs.SendingVerificationEmail;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Verification;
using Domain.Users;
using Domain.Users.Entities;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Numerics;
using System.Security.Principal;

namespace Application.Users.Register;


public static class RegisterErrors
{
    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static Error UserRegistrationFailed(string message) => Error.Failure(
        "Users.UserRegistrationFailed",
        message);
}
internal sealed class RegisterCommandHandler(UserManager<User> userManager,
                                             IUnitOfWork unitOfWork,
                                             ILogger<RegisterCommandHandler> logger)
    : ICommandHandler<RegisterCommand>
{

    public async Task<Result> Handle(RegisterCommand command,
                                     CancellationToken cancellationToken)
    {

        logger.LogInformation("Handling RegisterCommand for email: {Email}", command.Email);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        if (await userManager.FindByEmailAsync(command.Email) != null)
        {
            logger.LogWarning("Attempt to register user with non-unique email: {Email}", command.Email);
            return Result.Failure(RegisterErrors.EmailNotUnique);

            // TODO : for improvement :
            // if already  exists user with this email,
            // then we can simulate some work to make it look like  user dosent exists
            // and send email to user :
            // "Someone tried to create an account with your email.
            // If this was you, please sign in instead. If not, you can ignore this email."
        }

        logger.LogInformation("Registering user with email: {Email}", command.Email);


        User user = User.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            command.ProfilePictureSource
        );

        try
        {

            IdentityResult result = await userManager.CreateAsync(user, command.Password);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to register user with email: {Email}. Errors: {Errors}", command.Email, result.Errors);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(RegisterErrors.UserRegistrationFailed(string.Join(", ", result.Errors.Select(e => e.Description))));
            }
            logger.LogInformation("User registered successfully with email: {Email}", command.Email);


            user.Raise(new UserRegisteredDomainEvent(user.Id));
            logger.LogInformation("UserRegisteredDomainEvent raised for user with ID: {UserId}", user.Id);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {

            logger.LogError(ex, "An error occurred while registering user with email: {Email}", command.Email);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(RegisterErrors.UserRegistrationFailed("An unexpected error occurred during registration."));
        }


        return Result.Success();

        // TODO : in front end : 
        // Welcome! Please confirm your email to complete registration
    }


}
