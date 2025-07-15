using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;
using Application.Users.Profile.SocialLinks;

namespace Web.Api.Endpoints.Users.Profile;

internal sealed class UpdateSocialLinks : IEndpoint
{
    public sealed record Request(
        string? LinkedIn,
        string? Twitter,
        string? Github,
        string? Youtube,
        string? Facebook,
        string? Instagram,
        string? Portfolio);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile/social-links", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateSocialLinksCommand> handler,
            CancellationToken cancellationToken) =>
        {
            int userId = userContext.UserId;


            var command = new UpdateSocialLinksCommand(
                userId,
                request.LinkedIn,
                request.Twitter,
                request.Github,
                request.Youtube,
                request.Facebook,
                request.Instagram,
                request.Portfolio);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Profile);
    }
}
