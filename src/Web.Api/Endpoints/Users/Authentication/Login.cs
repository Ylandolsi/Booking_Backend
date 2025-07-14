using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Application.Users.Login;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.Login, async (
            Request request,
            ICommandHandler<LoginCommand, LoginResponse> handler,
            CancellationToken cancellationToken = default) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            Result<LoginResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
