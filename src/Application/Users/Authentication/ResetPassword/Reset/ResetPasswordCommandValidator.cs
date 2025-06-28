using FluentValidation;

namespace Application.Users.Authentication.ResetPassword.Send;

public class ResetPasswordCommandValidator : AbstractValidator<RestPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}