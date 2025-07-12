using Application.Abstractions.Messaging;
using Application.Users.Expertise.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Expertise;

internal sealed class GetAllExpertises : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetAllExpertises, async (
            IQueryHandler<GetAllExpertiseQuery, List<Domain.Users.Entities.Expertise>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllExpertiseQuery();
            Result<List<Domain.Users.Entities.Expertise>> result = await handler.Handle(query, cancellationToken);

            return result.Match((result) => Results.Ok(result), (result) => CustomResults.Problem(result));
        })
        .WithTags(Tags.Expertise);
    }
}
