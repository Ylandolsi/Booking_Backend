using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;

namespace Application.Users.GetUser;

public record GetUserQuery(string UserSlug) : IQuery<UserResponse>;