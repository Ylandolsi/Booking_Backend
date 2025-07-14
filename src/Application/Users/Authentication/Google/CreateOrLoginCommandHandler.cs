using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Security.Claims;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Authentication.Google;

internal sealed class CreateOrLoginCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    TokenHelper tokenHelper,
    ISlugGenerator slugGenerator,
    IApplicationDbContext context,
    ILogger<CreateOrLoginCommandHandler> logger) : ICommandHandler<CreateOrLoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(CreateOrLoginCommand command, CancellationToken cancellationToken)
    {
        ClaimsGoogle? claims = ExtractClaims(command.principal);

        if (claims is null)
        {
            return Result.Failure<LoginResponse>(
                CreateOrLoginErrors.UserRegistrationFailed("Invalid claims from external provider."));
        }

        var loginInfo = new UserLoginInfo("Google", claims.Id, "Google");

        // Find user by the external login first
        User? user = await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

        if (user is null)
        {
            // If not found by login, try by email
            user = await userManager.FindByEmailAsync(claims.Email);

            if (user is null)
            {
                // If user doesn't exist at all, create a new one
                logger.LogInformation("Creating new user with email {Email}.", claims.Email);

                string uniqueSlug = await slugGenerator.GenerateUniqueSlug(
                        async (slug) => await context.Users.AsNoTracking().AnyAsync(u => u.Slug == slug, cancellationToken),
                        claims.FirstName,
                        claims.LastName
                    );

                user = User.Create(
                    uniqueSlug,
                    claims.FirstName,
                    claims.LastName,
                    claims.Email,
                    claims.Picture ?? string.Empty);
                user.EmailConfirmed = true;

                IdentityResult createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    logger.LogWarning("Failed to register user with email: {Email}. Errors: {Errors}", claims.Email,
                        createResult.Errors);
                    return Result.Failure<LoginResponse>(
                        CreateOrLoginErrors.UserRegistrationFailed(string.Join(", ",
                            createResult.Errors.Select(e => e.Description))));
                }

                logger.LogInformation("User registered successfully with email: {Email}", claims.Email);
            }

            // Add the external login to the user (either existing by email or newly created)
            IdentityResult addLoginResult = await userManager.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                logger.LogWarning("Failed to add Google login to user with email: {Email}. Errors: {Errors}",
                    claims.Email, addLoginResult.Errors);
                return Result.Failure<LoginResponse>(
                    CreateOrLoginErrors.UserRegistrationFailed("Could not link Google account."));
            }
        }
        else
        {
            logger.LogInformation("User with email {Email} already exists, logging in.", claims.Email);
        }

        // At this point, 'user' is valid, so generate tokens and return the response
        // This part depends on your ITokenProvider implementation

        string? currentIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        string? currentUserAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        Result result = await tokenHelper.GenerateTokens(user, currentIp, currentUserAgent, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<LoginResponse>(result.Error);
        }

        logger.LogInformation("User {Email} logged in successfully.!", user.Email);

        var response = new LoginResponse
        (
            UserSlug: user.Slug,
            FirstName: user.Name.FirstName,
            LastName: user.Name.LastName,
            Email: user.Email!,
            ProfilePictureUrl: user.ProfilePictureUrl.ProfilePictureLink,
            IsMentor: user.Status.IsMentor,
            MentorActive: user.Status.IsMentor && user.Status.IsActive
        );

        return Result.Success(response);
    }

    private ClaimsGoogle? ExtractClaims(ClaimsPrincipal principal)
    {
        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("email")?.Value;
        var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value
                        ?? principal.FindFirst("given_name")?.Value
                        ?? principal.FindFirst("name")?.Value;
        var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value
                       ?? principal.FindFirst("family_name")?.Value
                       ?? ""; // fallback to empty if not present
        var picture = principal.FindFirst("picture")?.Value;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName))
        {
            return null;
        }

        return new ClaimsGoogle(id, email, firstName, lastName, picture);
    }

    private record ClaimsGoogle(string Id, string Email, string FirstName, string LastName, string? Picture);
}