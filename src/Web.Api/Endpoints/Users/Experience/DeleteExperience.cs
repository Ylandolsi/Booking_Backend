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
        app.MapDelete(UsersEndpoints.DeleteExperience, async (
            int experienceId,
            IUserContext userContext,
            ICommandHandler<DeleteExperienceCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;
            
    
            var command = new DeleteExperienceCommand(experienceId, userId);
            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Experience);
    }
}
