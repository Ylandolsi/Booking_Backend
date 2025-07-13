using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Delete;

public sealed record DeleteExperienceCommand(int ExperienceId, int UserId) : ICommand;


