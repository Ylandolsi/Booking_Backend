using Application.Abstractions.Messaging;

namespace Application.Users.Education.Add;

public sealed record AddEducationCommand(string Field,
                                          Guid UserId,
                                          string University,
                                          DateTime StartDate,
                                          DateTime? EndDate,
                                          string? Description) : ICommand<Guid>;