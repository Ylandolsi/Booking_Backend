using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Me;
using Application.Users.Authentication.Utils;
using Domain.Users.Entities;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class Me : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetCurrentUser, async (
            IUserContext userContext,
            IQueryHandler<MeQuery, MeData> handler,
            CancellationToken cancellationToken = default) =>
        {
            int userId = userContext.UserId;

            var query = new MeQuery(userId);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match((result) => Results.Ok(result), (result) => CustomResults.Problem(result));
        })

        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
