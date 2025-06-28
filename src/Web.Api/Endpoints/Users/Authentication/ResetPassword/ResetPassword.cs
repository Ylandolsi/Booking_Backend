using Application.Abstractions.Messaging;
using Application.Users.Authentication.ResetPassword.Verify;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication.ResetPassword;

internal sealed class ResetPassword : IEndpoint
{
    public sealed record Request(string Email, string Token, string Password, string ConfirmPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.ResetPassword, async (
            Request request,
            ICommandHandler<ResetPasswordCommand> handler,
            CancellationToken cancellationToken = default) =>
        {
            var command = new ResetPasswordCommand(
                request.Email,
                request.Token,
                request.Password,
                request.ConfirmPassword
            );

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.NoContent(),
                                (result) => CustomResults.Problem(result));
        })
        .WithTags(Tags.Users);
    }
}