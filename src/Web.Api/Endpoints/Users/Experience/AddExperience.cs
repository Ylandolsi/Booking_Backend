using Application.Abstractions.Messaging;
using Application.Users.Experience.Add;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;
using Application.Users.Education.Add;

namespace Web.Api.Endpoints.Users.Experience;

internal sealed class AddExperience : IEndpoint
{
    public sealed record Request(
        string Title,
        string Company,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.AddExperience, async (
            Request request,
            IUserContext userContext,
            ICommandHandler<AddExperienceCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;
            

            var command = new AddExperienceCommand(
                request.Title,
                userId,
                request.Company,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                (result) => Results.Ok(result),
                CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Experience);
    }
}