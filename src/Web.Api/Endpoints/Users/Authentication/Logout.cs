using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Logout;
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
            CancellationToken cancellationToken = default ) =>
        {
            Guid userId;
            try
            {
                userId = userContext.UserId;
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }

            var command = new LogoutCommand(userId);

            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
