using Domain.Users.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
}
