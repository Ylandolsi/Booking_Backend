using Application.Abstractions.Messaging;
using Application.Users.Experience.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users.Experience;


internal sealed class GetExperiences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetUserExperiences, async (
            string userSlug,
            IQueryHandler<GetExperienceQuery, List<GetExperienceResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetExperienceQuery(userSlug);
            Result<List<GetExperienceResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Experience);
    }
}