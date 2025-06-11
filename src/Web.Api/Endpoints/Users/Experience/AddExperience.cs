using Application.Abstractions.Messaging;
using Application.Users.Experience.Add;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;

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
        app.MapPost("users/experiences", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<AddExperienceCommand, Guid> handler,
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
            var command = new AddExperienceCommand(
                request.Title,
                userId,
                request.Company,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute("GetExperienceById", new { experienceId = id }, id), // Assuming a GetById endpoint exists or will be created
                CustomResults.Problem);
        })
        .WithTags(Tags.Experience);
    }
}