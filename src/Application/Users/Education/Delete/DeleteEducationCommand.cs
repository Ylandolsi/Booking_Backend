using Application.Abstractions.Messaging;

namespace Application.Users.Education.Delete;

public sealed record DeleteEducationCommand(int EducationId, int UserId) : ICommand; 


