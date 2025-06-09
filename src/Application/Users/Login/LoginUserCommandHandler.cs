using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Login;

public sealed class LoginUserCommandHandler
                (IApplicationDbContext context,
                IPasswordHasher passwordHasher,
                ITokenProvider tokenProvider,
                ILogger<LoginUserCommandHandler> logger) : ICommandHandler<LoginUserCommand, string>
{

    public async Task<Result<string>> Handle(LoginUserCommand command,
                         CancellationToken cancellationToken = default)
    {
        // check if email exists :
        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress.Email == command.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            // group them together for better security ( to avoid timing attacks )
            // and to avoid leaking information about whether the email exists or not
            logger.LogWarning("Login attempt failed for email : {Email}", command.Email);
            return Result.Failure<string>(UserErrors.IncorrectEmailOrPassword);
        }


        // generate JWT token for the user :
        string? token = tokenProvider.Create(user);
        if (token is null)
        {
            logger.LogError("Failed to generate token for user with email: {Email}", command.Email);
            return Result.Failure<string>(UserErrors.TokenGenerationFailed);
        }

        logger.LogInformation("User {Email} logged in successfully", command.Email);
        return token;  // automatically wrapped in Result<string> by the Result type 

    }

}