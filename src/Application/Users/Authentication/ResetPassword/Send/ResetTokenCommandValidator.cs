using FluentValidation;

namespace Application.Users.Authentication.ResetPassword.Send;

public class ResetTokenCommandValidator : AbstractValidator<ResetTokenCommand>
{
    public ResetTokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}