using FluentValidation;

namespace Application.Users.Experience.Add;

internal sealed class AddExperienceCommandValidator : AbstractValidator<AddExperienceCommand>
{
    public AddExperienceCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Company).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.EndDate)
            .GreaterThanOrEqualTo(c => c.StartDate)
            .When(c => c.EndDate.HasValue);

    }

}

