using Application.Abstractions.Messaging;
using Application.Users.Register;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class Register : IEndpoint
{

    public sealed record Request(string FirstName,
                                 string LastName,
                                 string Email,
                                 string Password,
                                 string ProfilePictureSource);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.Register, async (
            Request request,
            ICommandHandler<RegisterCommand> handler,
            CancellationToken cancellationToken = default) =>
        {

            var command = new RegisterCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.ProfilePictureSource
            );

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.Created(),
                               (result) => CustomResults.Problem(result));
        })
        .WithTags(Tags.Users);
    }
}
