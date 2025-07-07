using FluentValidation;

namespace Application.Users.Profile.SocialLinks;

internal sealed class UpdateSocialLinksCommandValidator : AbstractValidator<UpdateSocialLinksCommand>
{
    public UpdateSocialLinksCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.LinkedIn).MaximumLength(256);
        RuleFor(c => c.Twitter).MaximumLength(256);
        RuleFor(c => c.Github).MaximumLength(256);
        RuleFor(c => c.Youtube).MaximumLength(256);
        RuleFor(c => c.Facebook).MaximumLength(256);
        RuleFor(c => c.Instagram).MaximumLength(256);
        RuleFor(c => c.Portfolio).MaximumLength(256);
    }
}