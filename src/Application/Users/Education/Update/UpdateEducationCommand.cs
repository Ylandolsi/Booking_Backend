using Application.Abstractions.Messaging;

namespace Application.Users.Education.Update;

public sealed record UpdateEducationCommand(int EducationId,
                                            int UserId,
                                            string Field,
                                            string University,
                                            DateTime StartDate,
                                            DateTime? EndDate,
                                            string? Description) : ICommand;