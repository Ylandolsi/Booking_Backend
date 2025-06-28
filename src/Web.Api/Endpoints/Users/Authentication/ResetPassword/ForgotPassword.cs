using Application.Abstractions.Messaging;
using Application.Users.Authentication.ResetPassword.Send;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication.ResetPassword;

internal sealed class ForgotPassword : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.ForgotPassword, async (
            Request request,
            ICommandHandler<RestPasswordCommand> handler,
            CancellationToken cancellationToken = default) =>
        {
            var command = new RestPasswordCommand(request.Email);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.NoContent(),
                                (result) => CustomResults.Problem(result));
        })
        .WithTags(Tags.Users);
    }
}
