namespace Application.Users.Login;

public sealed record LoginResponse(
    Guid UserId,
    string Firstname,
    string Lastname,
    string Email,
    string AccessToken,
    bool IsMentor,
    string? ProfilePictureUrl = null,
    bool MentorActive = false);