using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Logout;
using SharedKernel.Exceptions;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class Logout : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.Logout, async (
            IUserContext userContext,
            ICommandHandler<LogoutCommand, bool> handler,
            CancellationToken cancellationToken = default) =>
        {
            int userId = userContext.UserId;
            var command = new LogoutCommand(userId);

            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
