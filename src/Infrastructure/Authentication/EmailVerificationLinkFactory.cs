using Application.Abstractions.Authentication;
using Application.Options;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Web;

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


        // Ensure the token is properly encoded for URL usage
        // cuz it contains characters that are not safe for URL ( + , / , = .. )
        //string encodedToken = Uri.EscapeDataString(emailVerificationToken);
        //string verificationLink = $"{_frontEndOptions.BaseUrl}{_frontEndOptions.EmailVerification}?token={encodedToken}&email={emailAdress}";
        //return verificationLink ?? throw new InvalidOperationException("Failed to generate verification link.");

        var builder = new UriBuilder(_frontEndOptions.BaseUrl)
        {
            Path = _frontEndOptions.EmailVerification.Trim('/'),
            Query = $"token={HttpUtility.UrlEncode(emailVerificationToken)}&email={HttpUtility.UrlEncode(emailAdress)}"
        };
        var resetUrl = builder.ToString();

        return resetUrl ?? throw new InvalidOperationException("Failed to generate email verification link. The link is null or empty.");

    }


}