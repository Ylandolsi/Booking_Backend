using Application.Abstractions.Messaging;


namespace Application.Users.Authentication.ResetPassword.Verify;

public record VerifyResetPasswordCommand(string Email,
                                          string Token,
                                          string Password,
                                          string ConfirmPassword) : ICommand;
