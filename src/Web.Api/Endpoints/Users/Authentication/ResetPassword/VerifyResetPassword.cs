using Application.Abstractions.Messaging;
using Application.Users.Authentication.ResetPassword.Verify;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication.ResetPassword;

internal sealed class VerifyResetPassword : IEndpoint
{
    public sealed record Request(string Email, string Token, string Password, string ConfirmPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.ResetPasswordVerify, async (
            Request request,
            ICommandHandler<VerifyResetPasswordCommand> handler,
            CancellationToken cancellationToken = default) =>
        {
            var command = new VerifyResetPasswordCommand(
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