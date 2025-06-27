using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Application.Abstractions.Authentication;
using Application.Users.RefreshAccessToken;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class RefreshAccessToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.RefreshAccessToken, async (
            IUserContext userContext,
            ICommandHandler<RefreshAccessTokenCommand> handler,
            CancellationToken cancellationToken = default ) =>
        {
            var refreshToken = userContext.RefreshToken;


            if (string.IsNullOrEmpty(refreshToken))
            {
                return CustomResults.Problem(Result.Failure<string>(
                    Error.Unauthorized("RefreshToken.Missing", "The refresh token was not found in the cookies.")));
            }

            var command = new RefreshAccessTokenCommand(refreshToken);
            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() =>Results.Created(), (_) => CustomResults.Problem(result) ) ;
        })
        .WithTags(Tags.Users);
    }
}

