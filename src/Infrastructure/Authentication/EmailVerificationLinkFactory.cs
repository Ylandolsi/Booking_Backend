using Application.Abstractions.Authentication;
using Application.Options;
using Microsoft.Extensions.Options;
using System.Web;

namespace Infrastructure.Authentication;

internal sealed class EmailVerificationLinkFactory(IOptions<FrontendApplicationOptions> frontEndOptions) : IEmailVerificationLinkFactory
{
    private readonly FrontendApplicationOptions _frontEndOptions = frontEndOptions.Value;
    public string Create(string emailVerificationToken, string emailAdress)
    {

        // Ensure the token is properly encoded for URL usage
        // cuz it contains characters that are not safe for URL ( + , / , = .. )

        var builder = new UriBuilder(_frontEndOptions.BaseUrl)
        {
            Path = _frontEndOptions.EmailVerification , 
            Query = $"token={HttpUtility.UrlEncode(emailVerificationToken)}&email={HttpUtility.UrlEncode(emailAdress)}"
        }; 
        var resetUrl = builder.ToString();

        return resetUrl ?? throw new InvalidOperationException("Failed to generate email verification link. The link is null or empty.");

    }


}