using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;
using Domain.Users.Entities;

namespace Application.Users.Authentication.Me;

public record MeQuery(int Id) : IQuery<MeData>;

