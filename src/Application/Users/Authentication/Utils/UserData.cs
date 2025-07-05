namespace Application.Users.Authentication.Utils;

public sealed record UserData(Guid UserId,
                              string FirstName,
                              string LastName,
                              string Email,
                              bool IsMentor,
                              string? ProfilePictureUrl = null,
                              bool MentorActive = false);