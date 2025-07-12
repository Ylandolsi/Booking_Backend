
using FluentValidation;

namespace Application.Users.Education.Update;

internal sealed class UpdateEducationCommandValidator : AbstractValidator<UpdateEducationCommand>
{
    public UpdateEducationCommandValidator()
    {
        RuleFor(c => c.EducationId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Field).NotEmpty().MaximumLength(100);
        RuleFor(c => c.University).NotEmpty().MaximumLength(100);
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.EndDate).GreaterThan(c => c.StartDate).When(c => c.EndDate.HasValue);
    }
}