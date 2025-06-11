using Application.Abstractions.Messaging;

namespace Application.Users.Experience.Add;

public sealed record AddExperienceCommand(string Title,
                                          Guid UserId , 
                                          string Company,
                                          DateTime StartDate,
                                          DateTime? EndDate,
                                          string? Description) : ICommand<Guid>;
