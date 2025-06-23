using Application.Abstractions.Messaging;
using Application.Users.Authentication.ResetPassword.Send;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication.ResetPassword;

internal sealed class SendResetToken : IEndpoint
{
    public sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.ResetPasswordSendToken, async (
            Request request,
            ICommandHandler<ResetTokenCommand> handler,
            CancellationToken cancellationToken = default) =>
        {
            var command = new ResetTokenCommand(request.Email);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.Ok(),
                                (result) => CustomResults.Problem(result));
        })
        .WithTags(Tags.Users);
    }
}
