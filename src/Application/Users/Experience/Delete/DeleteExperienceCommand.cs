using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Delete;

public sealed record DeleteExperienceCommand(Guid ExperienceId, Guid UserId) : ICommand<Guid>;


