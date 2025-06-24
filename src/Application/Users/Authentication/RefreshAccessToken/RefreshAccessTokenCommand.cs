using Application.Abstractions.Messaging;
using System.Threading.RateLimiting;


namespace Application.Users.RefreshAccessToken; 
public record RefreshAccessTokenCommand(string RefreshToken) : ICommand;
