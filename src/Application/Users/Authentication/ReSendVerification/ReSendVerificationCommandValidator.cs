using FluentValidation;

namespace Application.Users.ReSendVerification;

internal sealed class ReSendVerificationCommandValidator : AbstractValidator<ReSendVerificationEmailCommand>
{
    public ReSendVerificationCommandValidator()
    {        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID cannot be empty.");
    }
}