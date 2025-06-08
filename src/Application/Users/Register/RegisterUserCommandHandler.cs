using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class RegisterUserCommandHandler(IApplicationDbContext context,
            IPasswordHasher passwordHasher , 
            IEmailVerificationLinkFactory emailVerificationLinkFactory , 
            IFluentEmail fluentEmail , 
            ILogger<RegisterUserCommandHandler> logger)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.EmailAddress.Email == command.Email, cancellationToken))
        {
            logger.LogWarning("Attempt to register user with non-unique email: {Email}", command.Email);
            return Result.Failure<Guid>( new ValidationError( UserErrors.EmailNotUnique ) );
        }

        logger.LogInformation("Registering user with email: {Email}", command.Email);
        var hashedPassword = passwordHasher.Hash(command.Password);
        logger.LogInformation("Password for user {Email} hashed successfully", command.Email);

        var user = User.Create(
            command.FirstName, 
            command.LastName,
            command.Email,
            hashedPassword,
            command.ProfilePictureSource,
            command.IsMentor
        );
        if (user.IsFailure)
        {
            logger.LogError("Failed to create user with email: {Email}. Error: {Error}", command.Email, user.Error);
            return Result.Failure<Guid>(user.Error);
        }

        EmailVerificationToken emailVerificationToken;
        try
        {
            context.Users.Add(user.Value);
            emailVerificationToken = new EmailVerificationToken(user.Value.Id);
            context.EmailVerificationTokens.Add(emailVerificationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to register user with email: {Email}", command.Email);
            return Result.Failure<Guid>(UserErrors.UserRegistrationFailed(ex.Message));
        }

        string verificationEmailLink = emailVerificationLinkFactory.Create(emailVerificationToken); 
        logger.LogInformation("Sending verification email to {Email}", user.Value.EmailAddress.Email);
        
        
        await fluentEmail
            .To(user.Value.EmailAddress.Email)
            .Subject("Verify your email address")
            .Body($"To verify your email address <a href ='{verificationEmailLink}' > " +
                  $"click here</a> or copy and paste the following link into your browser:" +
                  $" {verificationEmailLink}", isHtml: true)
            .SendAsync(cancellationToken);

        logger.LogInformation("Verification email sent to {Email}", user.Value.EmailAddress.Email);
        
        // raise it after email is sent ! 
        // user.Value.Raise(new UserRegisteredDomainEvent(user.Value.Id));


        return user.Value.Id;
    }
}
