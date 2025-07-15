using FluentValidation;

namespace Application.Users.Profile.BasicInfo;

internal sealed class UpdateBasicInfoCommandValidator : AbstractValidator<UpdateBasicInfoCommand>
{
    public UpdateBasicInfoCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Bio).MaximumLength(500);
        RuleFor(c => c.Gender)
            .NotEmpty()
            .Must(g => g == "male" || g == "female")
            .WithMessage("Gender must be 'male' or 'female'.");
    }
}