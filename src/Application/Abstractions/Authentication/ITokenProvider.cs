using Domain.Users.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
