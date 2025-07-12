using FluentValidation;
using System.Linq;

namespace Application.Users.Expertise.Update;

internal sealed class UpdateUserExpertiseCommandValidator : AbstractValidator<UpdateUserExpertiseCommand>
{
    public UpdateUserExpertiseCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ExpertiseIds).NotNull();
        RuleFor(c => c.ExpertiseIds)
            .Must(ids => ids == null || ids.Count <= 4)
            .WithMessage("Maximum 4 expertise allowed");
        RuleFor(c => c.ExpertiseIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate expertise IDs are not allowed");
    }
}