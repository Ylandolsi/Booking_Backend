
using Application.Abstractions.Messaging;

namespace Application.Users.Expertise.Update;

public sealed record UpdateUserExpertiseCommand(Guid UserId, List<int>? ExpertiseIds) : ICommand;
