﻿namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    string? RefreshToken { get; }
}
