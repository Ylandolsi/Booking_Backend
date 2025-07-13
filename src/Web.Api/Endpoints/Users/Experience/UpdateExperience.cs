using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Users.Experience.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Experience;

internal sealed class UpdateExperience : IEndpoint

{
    public sealed record Request(
        string Title,
        string Company,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UsersEndpoints.UpdateExperience, async (
            int experienceId,
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateExperienceCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int userId;
            try
            {
                userId = userContext.UserId;
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }
            var command = new UpdateExperienceCommand(
                experienceId,
                userId, 
                request.Title,
                request.Company,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Experience);
    }
}
