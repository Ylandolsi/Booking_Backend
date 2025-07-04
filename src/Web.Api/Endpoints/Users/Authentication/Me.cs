using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Me;
using Application.Users.Authentication.Utils;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class Me : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetCurrentUser, async (
            IUserContext userContext,
            IQueryHandler<MeQuery, UserData> handler,
            CancellationToken cancellationToken = default) =>
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
            var query = new MeQuery(userId);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match((result) => Results.Ok(result), (result) => CustomResults.Problem(result));
        })

        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
