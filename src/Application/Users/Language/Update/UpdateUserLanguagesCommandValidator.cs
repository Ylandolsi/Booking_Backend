using FluentValidation;

namespace Application.Users.Languages.Update;

internal sealed class UpdateUserLanguagesCommandValidator : AbstractValidator<UpdateUserLanguagesCommand>
{
    public UpdateUserLanguagesCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.LanguageIds).NotNull();
        RuleFor(c => c.LanguageIds).Must(ids => ids.Count <= 4)
            .WithMessage("Maximum 4 languages allowed");
        RuleFor(c => c.LanguageIds).Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate language IDs are not allowed");
    }
}