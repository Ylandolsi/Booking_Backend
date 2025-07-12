using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Abstractions.Authentication;
using Application.Users.Languages.Update;

namespace Web.Api.Endpoints.Users.Language;

internal sealed class UpdateLanguage : IEndpoint
{
    public sealed record Request(List<int>? LanguageIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(UsersEndpoints.UpdateUserLanguages, async (
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateUserLanguagesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Guid userId;
            try
            {
                userId = userContext.UserId;
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }

            var command = new UpdateUserLanguagesCommand(userId, request.LanguageIds);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Language);
    }
}
