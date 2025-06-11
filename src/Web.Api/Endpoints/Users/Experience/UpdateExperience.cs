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
        app.MapPut("users/experiences/{experienceId:guid}", async (
            Guid experienceId,
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateExperienceCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateExperienceCommand(
                experienceId,
                request.Title,
                userContext.UserId,
                request.Company,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Experience);
    }
}
