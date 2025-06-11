using Application.Abstractions.Messaging;
using Application.Users.Experience.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;

namespace Web.Api.Endpoints.Users.Experience;

internal sealed class DeleteExperience : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("users/experiences/{experienceId:guid}", async (
            Guid experienceId,
            IUserContext userContext,
            ICommandHandler<DeleteExperienceCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Guid userId ;
            try
            {
                userId = userContext.UserId;
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }

            var command = new DeleteExperienceCommand(experienceId , userId);
            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Experience);
    }
}
