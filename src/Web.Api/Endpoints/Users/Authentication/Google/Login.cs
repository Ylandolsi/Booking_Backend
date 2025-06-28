
using Domain.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.Users.Authentication.Google;

internal sealed class LoginGoogle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GoogleLogin, async (
            [FromQuery] string? returnUrl,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<User> signInManager) =>
        {

            if (returnUrl is null)
            {
                returnUrl = "/";
            }

            string callbackEndpoint = linkGenerator.GetUriByName(httpContextAccessor.HttpContext!,
                                                                 UsersEndpoints.GoogleLoginCallback,
                                                                 new { returnUrl = returnUrl })!;



            // internally : 
            // * .net identity send the request including the scopes
            // * .net identity have its callback that change the auth code with token which is ( signin-google ) 
            // * we have our "so called" own callback which called after the callback of identity already called 
            // * we extract the calims from there and tokens 
            // and voila 

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties("Google",
                                                                                                      callbackEndpoint);

            properties.SetParameter("prompt", "select_account");
            // send a request to google with the properties ( including scope , state  , clientid ..... ) 
            // to get the code 
            return Results.Challenge(properties, new[] { "Google" });
        })
        .WithTags(Tags.Users)
        .WithName("GoogleLogin");

    }
}