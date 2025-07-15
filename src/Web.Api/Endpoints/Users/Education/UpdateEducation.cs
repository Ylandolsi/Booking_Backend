
using Application.Abstractions.Messaging;
using Application.Users.Education.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;

namespace Web.Api.Endpoints.Users.Education;

internal sealed class UpdateEducation : IEndpoint
{
    public sealed record Request(
        string Field,
        string University,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UsersEndpoints.UpdateEducation, async (
            int educationId,
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateEducationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int  userId = userContext.UserId;
            

            var command = new UpdateEducationCommand(
                educationId,
                userId,
                request.Field,
                request.University,
                request.StartDate,
                request.EndDate,
                request.Description);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent,
                                CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Education);
    }
}
