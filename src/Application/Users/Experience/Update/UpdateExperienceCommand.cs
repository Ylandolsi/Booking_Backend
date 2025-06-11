using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Update;

public sealed record UpdateExperienceCommand(Guid Id,
                                             string Title,
                                             Guid UserId,
                                             string Company,
                                             DateTime StartDate,
                                             DateTime? EndDate,
                                             string? Description) : ICommand<Guid>;
