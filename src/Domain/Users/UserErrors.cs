﻿using System.Data;
using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFoundById(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error NotFoundByEmail(string email) => Error.NotFound(
        "Users.NotFoundByEmail",
        $"The user with the email = '{email}' was not found");


    public static readonly Error EmailIsNotVerified = Error.Problem(
        "Users.EmailIsNotVerified",
        "The email address is not verified. Please verify your email before proceeding.");

    public static readonly Error IncorrectEmailOrPassword = Error.Problem(
    "Users.IncorrectEmailOrPassword",
    "The provided email or password is incorrect. Please try again.");

    public static readonly Error AccountLockedOut = Error.Unauthorized(
        "Users.AccountLockedOut",
        "This account has been locked out due to too many failed login attempts. Please try again later.");

    public static Error Unauthorized() => Error.Unauthorized(
    "Users.Unauthorized",
    "You are not authorized to perform this action.");



}
