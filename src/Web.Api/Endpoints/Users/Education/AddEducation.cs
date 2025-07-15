using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;
using Application.Users.Education.Add;

namespace Web.Api.Endpoints.Users.Education;

internal sealed class AddEducation : IEndpoint
{
    public sealed record Request(
        string Field,
        string University,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(UsersEndpoints.AddEducation, async (
            Request request,
            IUserContext userContext,
            ICommandHandler<AddEducationCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;



            var command = new AddEducationCommand(
                request.Field,
                userId,
                request.University,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                (result) => Results.Ok(result),
                CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Education);
    }
}

