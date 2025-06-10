namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    bool IsEmailVerified { get; }
    string? RefreshToken { get; }
}
