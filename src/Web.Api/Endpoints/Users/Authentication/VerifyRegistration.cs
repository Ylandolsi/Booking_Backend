using Application.Abstractions.Messaging;
using Application.Users.Authentication.Verification.VerifyEmail;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;



namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class VerifyRegistration : IEndpoint
{
    // [AsParameters]
    public sealed record Request(string Email, string Token);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.VerifyEmail, async ([FromBody] Request request,
                                                ICommandHandler<VerifyEmailCommand> handler,
                                                CancellationToken cancellationToken = default) =>
            {
                var command = new VerifyEmailCommand(request.Email, request.Token);
                Result result = await handler.Handle(command, cancellationToken);
                return result.Match(
                        () => Results.Created(), // 201 no return data 
                        (_) => CustomResults.Problem(result));
            })
        .WithTags(Tags.Users)
        .WithName(EndpointsNames.verifyEmail); // to get the endpoint by name and use it in the handler (EmailVerificationLinkFactory)
    }

}
