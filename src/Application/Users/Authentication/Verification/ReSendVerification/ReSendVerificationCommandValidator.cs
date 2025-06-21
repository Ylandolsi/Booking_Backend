using FluentValidation;

namespace Application.Users.ReSendVerification;

internal sealed class ReSendVerificationCommandValidator : AbstractValidator<ReSendVerificationCommand>
{
    public ReSendVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }
}