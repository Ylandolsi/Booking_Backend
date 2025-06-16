using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Experience.Delete;

public sealed record DeleteExperienceCommand (Guid ExperienceId , Guid UserId ) : ICommand<Guid>;


