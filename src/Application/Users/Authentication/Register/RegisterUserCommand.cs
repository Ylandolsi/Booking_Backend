using Application.Abstractions.Messaging;

namespace Application.Users.Register;

public sealed record RegisterUserCommand(string FirstName,
                                         string LastName,
                                         string Email,
                                         string Password,
                                         string ProfilePictureSource) : ICommand;



