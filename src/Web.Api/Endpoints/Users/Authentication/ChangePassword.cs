using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.ChangePassword;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Authentication;

internal sealed class ChangePassword : IEndpoint
{
    public sealed record Request(
        string OldPassword,
        string NewPassword,
        string ConfirmNewPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UsersEndpoints.ChangePassword, async (
            Request request,
            IUserContext userContext,
            ICommandHandler<ChangePasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Guid userId;
            try
            {
                userId = userContext.UserId;
            }
            catch (ApplicationException)
            {
                return Results.Unauthorized();
            }

            var command = new ChangePasswordCommand(
                userId,
                request.OldPassword,
                request.NewPassword,
                request.ConfirmNewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.Ok(),
                CustomResults.Problem);
        })
        // .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}