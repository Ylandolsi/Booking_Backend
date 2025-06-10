using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.VerifyEmail;

public class VerifyEmailCommandHandler(
    IApplicationDbContext context,
    ILogger<VerifyEmailCommandHandler> logger,
    ITokenProvider tokenProvider, 
    ITokenWriterCookies tokenWriterCookies) : ICommandHandler<VerifyEmailCommand, bool>
{
    public async Task<Result<bool>> Handle(VerifyEmailCommand command, CancellationToken cancellationToken = default)
    {
        EmailVerificationToken? token = await context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == command.Token, cancellationToken); 
        

        if (token is null)
        {
            logger.LogWarning("Email verification token not found for token ID: {TokenId}", command.Token);
            return Result.Failure<bool>(Error.Problem("User.Email.Verification.Token.NotFound",
                "Email verification token not found."));
        }
        
        if (token.User.EmailAddress.IsVerified())
        {
            logger.LogInformation("Email address {Email} is already verified.", token.User.EmailAddress.Email);
            return Result.Failure<bool>(Error.Problem("User.Email.Already.Verified", "Email address is already verified."));
        }

        if (!token.IsStillValid())
        {
            logger.LogWarning("Email verification token expired for token ID: {TokenId}", command.Token);
            return Result.Failure<bool>(Error.Problem("User.Email.Verification.Token.Expired",
                "Email verification token has expired."));
        }
        logger.LogInformation("Verifying email address for user ID: {UserId}", token.User.Id);
        
        token.User.EmailAddress.Verify();
        context.EmailVerificationTokens.Remove(token);
        try 
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to verify email for user ID: {UserId}", token.User.Id);
            return Result.Failure<bool>(UserErrors.EmailVerificationFailed(ex.Message));
        }

        logger.LogInformation("Email address {Email} verified successfully for user ID: {UserId}", token.User.EmailAddress.Email, token.User.Id);
        
        // update the JWTaccesstoken ( EmailVerified Claim now ) 
        string updatedJwtAccessToken = tokenProvider.GenrateJwtToken(token.User);
        tokenWriterCookies.WriteAccessTokenAsHttpOnlyCookie(updatedJwtAccessToken);
        logger.LogInformation("Updated JWT access token for user ID: {UserId}", token.User.Id);

        return Result.Success<bool>(true);
    }
    
}