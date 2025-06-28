using Application.Abstractions.Messaging;


namespace Application.Users.Authentication.ResetPassword.Verify;

public record ResetPasswordCommand(string Email,
                                          string Token,
                                          string Password,
                                          string ConfirmPassword) : ICommand;
