namespace Application.Users.Login;

public sealed record LoginUserResponse(
    Guid UserId,
    string Firstname,
    string Lastname,
    string Email,
    string AccessToken,
    bool IsMentor,
    string? ProfilePictureUrl = null,
    bool MentorActive = false);