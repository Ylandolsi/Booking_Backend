using Application.Abstractions.Authentication;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;

namespace Infrastructure.Authentication;

internal sealed class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor,
                        LinkGenerator linkGenerator) : IEmailVerificationLinkFactory
{
    public string Create(EmailVerificationToken emailVerificationToken)
    {

        // TODO : Change the URL To the actual URL of the application ( front end)
        // and from there w'ill send the request to the verify email page
        // https://yourfrontend.com/verify-email?token=<verification_token_guid> 
        string? verificationLink = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            EndpointsNames.verifyEmail,
            new { token = emailVerificationToken.Id })!; // Query Parameter 


        return verificationLink ?? throw new InvalidOperationException("Failed to generate verification link.");

    }
    // TODO : 
    // 1. Add a configuration section for the frontend application settings ( Option pattern )
    /*
        public class FrontendApplicationSettings
        {
            public const string SectionName = "FrontendApplication";
            public string BaseUrl { get; set; } = string.Empty;
            public string EmailVerificationPagePath { get; set; } = string.Empty; // e.g., "auth/verify-email"
        }

        "FrontendApplication": {
        "BaseUrl": "https://yourfrontend.com", // Replace with your actual frontend URL (e.g., http://localhost:3000 for dev)
        "EmailVerificationPagePath": "auth/verify-email" // Replace with your frontend's verification route
        },


        builder.Services.Configure<FrontendApplicationSettings>(
        builder.Configuration.GetSection(FrontendApplicationSettings.SectionName));

    */
    // 2. Use the configuration section to generate the verification link
    /* 
    
    internal sealed class EmailVerificationLinkFactory : IEmailVerificationLinkFactory
        {
        private readonly FrontendApplicationSettings _frontendSettings;
        // Remove LinkGenerator and IHttpContextAccessor if no longer needed for direct API link generation

        public EmailVerificationLinkFactory(IOptions<FrontendApplicationSettings> frontendSettingsOptions)
        {
            _frontendSettings = frontendSettingsOptions.Value;
        }

        public string Create(EmailVerificationToken token)
        {
            if (string.IsNullOrWhiteSpace(_frontendSettings.BaseUrl) || 
                string.IsNullOrWhiteSpace(_frontendSettings.EmailVerificationPagePath))
            {
                // Log this error appropriately
                throw new InvalidOperationException(
                    "Frontend base URL or email verification page path is not configured.");
            }

            // Ensure BaseUrl ends with a slash and EmailVerificationPagePath doesn't start with one for clean joining
            var baseUri = new Uri(_frontendSettings.BaseUrl.TrimEnd('/') + "/");
            var fullPath = new Uri(baseUri, $"{_frontendSettings.EmailVerificationPagePath.TrimStart('/')}?token={token.Id}");

            return fullPath.ToString();
        }
    */
    
}