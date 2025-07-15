using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using SharedKernel.Exceptions;

namespace Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new UnauthException("User context is unavailable");



    public string? RefreshToken =>
        _httpContextAccessor
            .HttpContext?
            .Request
            .Cookies["refresh_token"];


}
