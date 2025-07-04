
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Domain.Users;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Users.Authentication.Me;

public sealed class MeQueryHandler (UserManager<User> userManager,
                                    ILogger<MeQueryHandler> logger ) : IQueryHandler<MeQuery, UserData>
{
    public async Task<Result<UserData>> Handle(MeQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling MeQuery for user ID: {UserId}", query.Id);
        User? user = await userManager.FindByIdAsync(query.Id.ToString()); 
        
        if ( user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", query.Id);
            return Result.Failure<UserData>(UserErrors.NotFoundById(query.Id));
        }

        var response = new UserData
        (
            UserId: user.Id,
            Firstname: user.Name.FirstName,
            Lastname: user.Name.LastName,
            Email: user.Email!,
            ProfilePictureUrl: user.ProfilePictureUrl.ProfilePictureLink,
            IsMentor: user.Status.IsMentor,
            MentorActive: user.Status.IsMentor && user.Status.IsActive
        );

        return Result.Success(response);



    }
}
