using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Profile.ProfilePicture;

public record UpdateProfilePictureCommand(Guid UserId, IFormFile File) : ICommand<string>;