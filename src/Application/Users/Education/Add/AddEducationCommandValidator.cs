using FluentValidation;

namespace Application.Users.Education.Add;

internal sealed class AddEducationCommandValidator : AbstractValidator<AddEducationCommand>
{
    public AddEducationCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Field).NotEmpty().MaximumLength(50);
        RuleFor(c => c.University).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.EndDate).GreaterThan(c => c.StartDate).When(c => c.EndDate.HasValue);
    }
}