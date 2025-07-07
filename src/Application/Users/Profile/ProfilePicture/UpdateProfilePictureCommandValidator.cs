using FluentValidation;

namespace Application.Users.Profile.ProfilePicture;

internal sealed class UpdateProfilePictureCommandValidator : AbstractValidator<UpdateProfilePictureCommand>
{
    public UpdateProfilePictureCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.File).NotNull().Must(file => file.Length > 0).WithMessage("File is required");
        RuleFor(c => c.File).Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("File size must not exceed 5MB");
        RuleFor(c => c.File).Must(file =>
            file.ContentType.StartsWith("image/")).WithMessage("File must be an image");
    }
}