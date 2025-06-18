using Application.Abstractions.Messaging;

namespace Application.Users.Authentication.Verification.VerifyEmail;


public record VerifyEmailCommand( string Email, string Token) : ICommand; 