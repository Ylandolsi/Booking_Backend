
using Application.Abstractions.Messaging;

namespace Application.Users.Expertise.Update;

public sealed record UpdateUserExpertiseCommand(int UserId, List<int>? ExpertiseIds) : ICommand;
