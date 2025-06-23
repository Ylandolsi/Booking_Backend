using Application.Abstractions.Messaging;
using FluentValidation;

namespace Application.Users.Register;

public sealed record RegisterCommand(string FirstName,
                                     string LastName,
                                     string Email,
                                     string Password,
                                     string ProfilePictureSource) : ICommand;




