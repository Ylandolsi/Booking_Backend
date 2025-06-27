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
    public sealed record Request(string Email);


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.ResendVerificationEmail, async (
                Request request,
                ICommandHandler<ReSendVerificationCommand> handler,
                CancellationToken cancellationToken = default) =>
            {
                var command = new ReSendVerificationCommand(request.Email);
                Result result = await handler.Handle(command, cancellationToken);
                return result.Match(
                    () => Results.Created(),
                    (_) => CustomResults.Problem(result)
                );
            })
            .WithTags(Tags.Users);
    }
}