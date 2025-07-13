using Application.Abstractions.Messaging;
using Domain.Users.Entities;

namespace Application.Users.Profile.BasicInfo;

public record UpdateBasicInfoCommand(int UserId,
                                     string FirstName,
                                     string LastName,
                                     Genders Gender,
                                     string? Bio) : ICommand;
