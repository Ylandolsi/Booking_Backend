using Application.Abstractions.Messaging;
using Application.Users.Profile.BasicInfo;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;
using Domain.Users.Entities;

namespace Web.Api.Endpoints.Users.Profile;

internal sealed class UpdateBasicInfo : IEndpoint
{
    public sealed record Request(
        string FirstName,
        string LastName,
        string Gender,
        string? Bio);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile/basic-info", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateBasicInfoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;
     

            var command = new UpdateBasicInfoCommand(
                userId,
                request.FirstName,
                request.LastName,
                request.Gender,
                request.Bio);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
