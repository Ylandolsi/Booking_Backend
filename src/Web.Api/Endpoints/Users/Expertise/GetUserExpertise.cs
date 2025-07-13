using Application.Abstractions.Messaging;
using Application.Users.Expertise.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Expertise;

internal sealed class GetUserExpertise : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetUserExpertises, async (
            string userSlug,
            IQueryHandler<GetUserExpertisesQuery, List<Domain.Users.Entities.Expertise>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserExpertisesQuery(userSlug);
            Result<List<Domain.Users.Entities.Expertise>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Expertise);
    }
}
