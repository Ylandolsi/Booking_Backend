using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.GetUser;

internal sealed class GetUserQueryHandler(
    IApplicationDbContext context,
    ILogger<GetUserQueryHandler> logger
) : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving user data with slug = {Slug}", request.UserSlug);

        var user = await context.Users
            .AsNoTracking() // necessary to disable tracking
                            // for the entities inside the select  ,
                            // else error will happen: 
                            // bcz value objects are tracked while the main 
                            // entity is not tracked
            .Where(u => u.Slug == request.UserSlug).
            Include(u => u.Experiences)
           .Include(u => u.Educations)
           .Include(u => u.UserExpertises)
           .Include(u => u.UserLanguages)
           .Include(u => u.Experiences)
           .AsSplitQuery()
           .Select(u =>
               new UserResponse(
                   u.Slug, u.Name.FirstName, u.Name.LastName, u.Status, u.ProfilePictureUrl, u.Gender, u.SocialLinks,
                   u.Bio,
                   u.Experiences.ToList(), u.Educations.ToList(), u.UserExpertises.Select(ue => ue.Expertise).ToList(),
                   u.UserLanguages.Select(ul => ul.Language).ToList()
               )).FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User with Slug {UserSlug} not found", request.UserSlug);
            return Result.Failure<UserResponse>(UserErrors.NotFoundBySlug(request.UserSlug ?? "_"));
        }

        return Result.Success(user);
    }
}