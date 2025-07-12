using Domain.Users.Entities;
using SharedKernel;

namespace Domain.Users.ValueObjects;

public class ProfileCompletionStatus : ValueObject
{
    public bool HasName { get; private set; }
    public bool HasEmail { get; private set; }
    public bool HasProfilePicture { get; private set; }
    public bool HasBio { get; private set; }
    public bool HasGender { get; private set; }
    public bool HasSocialLinks { get; private set; }
    // public bool HasExperience { get; private set; }
    public bool HasEducation { get; private set; }
    public bool HasLanguages { get; private set; }
    public bool HasExpertise { get; private set; }

    public void UpdateCompletionStatus(User user)
    {
        HasName = user.Name is not null && !string.IsNullOrEmpty(user.Name.FirstName) && !string.IsNullOrEmpty(user.Name.LastName);
        HasEmail = !string.IsNullOrEmpty(user.Email);
        // it must be different from default image as well 
        HasProfilePicture = user.ProfilePictureUrl is not null && !string.IsNullOrEmpty(user.ProfilePictureUrl.ProfilePictureLink);
        HasBio = !string.IsNullOrEmpty(user.Bio);
        HasGender = user.Gender != default; // Assuming default value means not set
        HasSocialLinks = user.SocialLinks is not null;
        // HasExperience = user.Experiences.Any();
        HasEducation = user.Educations.Any();
        HasLanguages = user.UserLanguages.Any();
        HasExpertise = user.UserExpertises.Any();
    }

    public double GetCompletionPercentage()
    {
        int totalFields = 9;
        int completedFields = 0;

        if (HasName) completedFields++;
        if (HasEmail) completedFields++;
        if (HasProfilePicture) completedFields++;
        if (HasBio) completedFields++;
        if (HasGender) completedFields++;
        if (HasSocialLinks) completedFields++;
        // if (HasExperience) completedFields++;
        if (HasEducation) completedFields++;
        if (HasLanguages) completedFields++;
        if (HasExpertise) completedFields++;

        return (double)completedFields / totalFields * 100;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HasName;
        yield return HasEmail;
        yield return HasProfilePicture;
        yield return HasBio;
        yield return HasGender;
        yield return HasSocialLinks;
        // yield return HasExperience;
        yield return HasEducation;
        yield return HasLanguages;
        yield return HasExpertise;
    }
}