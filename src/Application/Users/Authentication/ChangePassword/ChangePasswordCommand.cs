using Application.Abstractions.Messaging;

namespace Application.Users.Authentication.ChangePassword;

public sealed record ChangePasswordCommand(
    int UserId,
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;
