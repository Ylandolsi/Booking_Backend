
using FluentValidation;

namespace Application.Users.Experience.Update;

internal sealed class UpdateExperienceCommandValidator : AbstractValidator<UpdateExperienceCommand>
{
    public UpdateExperienceCommandValidator()
    {
        RuleFor(c => c.ExperienceId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Company).NotEmpty().MaximumLength(100);
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.EndDate).GreaterThan(c => c.StartDate).When(c => c.EndDate.HasValue);
    }
}