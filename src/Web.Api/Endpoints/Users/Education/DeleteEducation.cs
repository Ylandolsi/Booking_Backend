using Application.Abstractions.Messaging;
using Application.Users.Education.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;

namespace Web.Api.Endpoints.Users.Education;

internal sealed class DeleteEducation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(UsersEndpoints.DeleteEducation, async (
            Guid educationId,
            IUserContext userContext,
            ICommandHandler<DeleteEducationCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Guid userId;
            try
            {
                userId = userContext.UserId;
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }

            var command = new DeleteEducationCommand(educationId, userId);
            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Education);
    }
}