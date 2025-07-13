using Application.Abstractions.Messaging;
using Application.Users.Education.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Abstractions.Authentication;

namespace Web.Api.Endpoints.Users.Education;

internal sealed class GetEducations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(UsersEndpoints.GetUserEducations, async (
            string userSlug ,
            IQueryHandler<GetEducationQuery, List<GetEducationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEducationQuery(userSlug);
            Result<List<GetEducationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                educations => Results.Ok(educations),
                CustomResults.Problem);
        })
        .WithTags(Tags.Education);
    }
}
