using Application.Abstractions.Authentication;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Authentication;

internal sealed class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor,
                                                   LinkGenerator linkGenerator,
                                                   IOptions<FrontendApplicationOptions> frontEndOptions) : IEmailVerificationLinkFactory
{
    private readonly FrontendApplicationOptions _frontEndOptions = frontEndOptions.Value;
    public string Create(string emailVerificationToken, string emailAdress)
    {


        //string? verificationLink = linkGenerator.GetUriByName(
        //    httpContextAccessor.HttpContext!,
        //    EndpointsNames.verifyEmail,
        //    new { token = emailVerificationToken, email = emailAdress })!; // Query Parameter 


        // Ensure the token is properly encoded for URL usage ( + , / , = .. )
        string encodedToken = Uri.EscapeDataString(emailVerificationToken);
        string verificationLink = $"{_frontEndOptions.BaseUrl}{_frontEndOptions.EmailVerificationPagePath}?token={encodedToken}&email={emailAdress}";
        return verificationLink ?? throw new InvalidOperationException("Failed to generate verification link.");

    }


}