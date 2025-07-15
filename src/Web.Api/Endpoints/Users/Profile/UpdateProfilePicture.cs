using Application.Abstractions.Messaging;
using Application.Users.Profile.ProfilePicture;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Web.Api.Endpoints.Users.Profile;

internal sealed class UpdateProfilePicture : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile/picture", async (
            IFormFile file,
            IUserContext userContext,
            ICommandHandler<UpdateProfilePictureCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            int  userId = userContext.UserId;
            


            var command = new UpdateProfilePictureCommand(userId, file);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                    Results.NoContent,
                    CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile)
        .DisableAntiforgery();
    }
}
