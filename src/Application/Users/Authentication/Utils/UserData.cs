namespace Application.Users.Authentication.Utils;

public sealed record UserData(
    Guid UserId,
    string Firstname,
    string Lastname,
    string Email,
    bool IsMentor,
    string? ProfilePictureUrl = null,
    bool MentorActive = false);