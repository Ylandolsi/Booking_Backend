using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.ReSendVerification;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class ReSendVerificationEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/resend-verification-email", async (
                IUserContext userContext,
                ICommandHandler<ReSendVerificationEmailCommand, bool> handler,
                CancellationToken cancellationToken = default) =>
            {
                Guid userId = Guid.Empty;
                try
                {
                    userId = userContext.UserId;
                }
                catch (Exception ex)
                {
                    return Results.Unauthorized();
                }

                var command = new ReSendVerificationEmailCommand(userId);
                Result<bool> result = await handler.Handle(command, cancellationToken);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Users)
            .RequireAuthorization();
    }
}