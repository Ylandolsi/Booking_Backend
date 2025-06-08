using Application.Abstractions.Authentication;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;

namespace Infrastructure.Authentication;

internal sealed class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor ,
                        LinkGenerator linkGenerator ) : IEmailVerificationLinkFactory
{
    public string Create(EmailVerificationToken emailVerificationToken)
    {

        string? verificationLink = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            EndpointsNames.verifyEmail , 
            new { token = emailVerificationToken.Id } )! ; // Query Parameter 
        
        
        return verificationLink ?? throw new InvalidOperationException("Failed to generate verification link.");

    }
    
    
    
}