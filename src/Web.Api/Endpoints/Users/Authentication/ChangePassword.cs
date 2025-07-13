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
            HttpContext httpContext,
            ICommandHandler<ChangePasswordCommand> handler,
            ILogger<ChangePassword> logger,
            CancellationToken cancellationToken) =>
        {
            int userId;
            try
            {
                userId = userContext.UserId;
                logger.LogInformation("Successfully retrieved user ID: {UserId}", userId);
            }
            catch (ApplicationException ex)
            {
                logger.LogWarning(ex, "Failed to get user ID in change password endpoint");
                return Results.Problem($"Failed to get user ID: {ex.Message}", statusCode: 401);
            }

            var command = new ChangePasswordCommand(
                userId,
                request.OldPassword,
                request.NewPassword,
                request.ConfirmNewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}