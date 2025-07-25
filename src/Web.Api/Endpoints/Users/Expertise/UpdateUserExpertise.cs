using Application.Abstractions.Messaging;
using Application.Users.Expertise.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;

namespace Web.Api.Endpoints.Users.Expertise;

internal sealed class UpdateUserExpertise : IEndpoint
{
    public sealed record Request(List<int>? ExpertiseIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UsersEndpoints.UpdateUserExpertise, async (
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateUserExpertiseCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;
            

            var command = new UpdateUserExpertiseCommand(userId, request.ExpertiseIds);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Expertise);
    }
}