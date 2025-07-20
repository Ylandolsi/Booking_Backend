using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Application.Users.GetUser;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class GetUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetUser,
            async (
                string userSlug,
                IQueryHandler<GetUserQuery, UserResponse?> handler, IUserContext userContext,
                CancellationToken cancellationToken) =>
            {
                int userId = userContext.UserId;
                var query = new GetUserQuery(userSlug);

                Result<UserResponse> result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok,
                                    CustomResults.Problem);
            });
    }
}