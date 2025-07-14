using Application.Abstractions.Messaging;
using Application.Users.Authentication.Google;
using Application.Users.Authentication.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Endpoints.Users.Authentication.Google;


internal sealed class LoginGoogleCallback : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GoogleLoginCallback, async (
            [FromQuery] string returnUrl,
            ICommandHandler<CreateOrLoginCommand, LoginResponse> createOrLoginCommandHandler,
            IHttpContextAccessor httpContextAccessor) =>
        {

            // exchange the code with tokens 
            // result.principal => claims
            // and result.ticket includes tokens and other stuff 
            AuthenticateResult result = httpContextAccessor.HttpContext != null
                ? await httpContextAccessor.HttpContext.AuthenticateAsync("Google")
                : AuthenticateResult.Fail("");

            if (!result.Succeeded)
            {
                Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Google login failed",
                    detail: "Authentication with Google was not successful. Please try again."
                );
            }


            var command = new CreateOrLoginCommand(result.Principal!);
            Result<LoginResponse> loginResponseResult = await createOrLoginCommandHandler.Handle(command, default);

            if (!loginResponseResult.IsSuccess)
            {
                Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Google login failed",
                    detail: "Authentication with Google was not successful. Please try again."
                );
            }


            
            return Results.Redirect(returnUrl);


        })
        .WithTags(Tags.Users)
        .WithName(UsersEndpoints.GoogleLoginCallback);

    }
}



//private async Task<GoogleTokenResponse> RefreshGoogleTokens(string refreshToken)
//    {
//        using var client = new HttpClient();
//        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
//        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
//        {
//               neksa l code 
//            ["client_id"] = _configuration["Google:ClientId"],
//            ["client_secret"] = _configuration["Google:ClientSecret"],
//            ["refresh_token"] = refreshToken,
//            ["grant_type"] = "refresh_token"
//        });

//        var response = await client.SendAsync(request);
//        if (!response.IsSuccessStatusCode)
//        {
//            return null;
//        }

//        return await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
//    }

//public class GoogleTokenResponse
//{
//    public string AccessToken { get; set; }
//    public string RefreshToken { get; set; }
//    public int ExpiresIn { get; set; }
//    public string TokenType { get; set; }
//}