using Application.Abstractions.Messaging;
using Application.Users.Authentication.Verification.VerifyEmail;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;



namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class VerifyRegistration : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/verify-email", async (
            [FromQuery] Guid token, ICommandHandler<VerifyEmailCommand, string> handler, CancellationToken cancellationToken = default) =>
            {
                var command = new VerifyEmailCommand(token);
                Result<string> result = await handler.Handle(command, cancellationToken);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
        .WithTags(Tags.Users)
        .WithName(EndpointsNames.verifyEmail); // to get the endpoint by name and use it in the handler (EmailVerificationLinkFactory)
    }

}
