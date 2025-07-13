using Application.Abstractions.Messaging;
using Application.Users.Authentication.Utils;

namespace Application.Users.Authentication.Me;

public record  MeQuery  ( int Id ): IQuery<UserData>; 

