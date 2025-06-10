using Domain.Users.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenrateJwtToken(User user);
    string GenerateRefreshToken();
}
