using Application.Abstractions.Messaging;

namespace Application.Users.Education.Delete;

public sealed record DeleteEducationCommand(Guid EducationId, Guid UserId) : ICommand<Guid>;


