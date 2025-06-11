using FluentValidation;

namespace Application.Users.Experience.Update;

internal sealed class UpdateExperienceCommandValidator : AbstractValidator<UpdateExperienceCommand>
{
    public UpdateExperienceCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Company).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
    }

}
