using SharedKernel;

namespace Domain.Users.ValueObjects;

public class ProfilePicture : ValueObject
{
    const string DefaultProfilePictureUrl = "https://www.pngarts.com/files/10/Default-Profile-Picture-PNG-Download-Image.png";
    public string ProfilePictureLink { get; private set; }
    public string ThumbnailUrlPictureLink { get; private set; } = DefaultProfilePictureUrl;

    public ProfilePicture(string profilePictureLink = "", string thumbnailUrlPictureLink = "")
    {
        if (string.IsNullOrWhiteSpace(profilePictureLink))
        {
            profilePictureLink = DefaultProfilePictureUrl;
        }

        if (!IsValidUrl(profilePictureLink!))
        {
            throw new ArgumentException("Invalid profile picture URL", nameof(profilePictureLink));
        }
        if (!string.IsNullOrWhiteSpace(thumbnailUrlPictureLink) && !IsValidUrl(thumbnailUrlPictureLink))
        {
            throw new ArgumentException("Invalid thumbnail URL", nameof(thumbnailUrlPictureLink));
        }

        ProfilePictureLink = profilePictureLink;
        ThumbnailUrlPictureLink = thumbnailUrlPictureLink;
    }


    public Result UpdateProfilePicture(string profilePictureLink, string thumbnailUrlPictureLink = "")
    {
        if (string.IsNullOrEmpty(profilePictureLink))
        {
            return ResetToDefaultProfilePicture();
        }
        if (!IsValidUrl(profilePictureLink))
        {
            return Result.Failure(ProfilePictureErrors.InvalidProfilePictureUrl);
        }
        if (!string.IsNullOrWhiteSpace(thumbnailUrlPictureLink) && !IsValidUrl(thumbnailUrlPictureLink))
        {
            return Result.Failure(ProfilePictureErrors.InvalidProfilePictureUrl);
        }

        ProfilePictureLink = profilePictureLink;
        ThumbnailUrlPictureLink = thumbnailUrlPictureLink;
        return Result.Success();
    }


    public Result ResetToDefaultProfilePicture()
    {
        ProfilePictureLink = DefaultProfilePictureUrl;
        ThumbnailUrlPictureLink = DefaultProfilePictureUrl;
        return Result.Success();
    }

    private static bool IsValidUrl(string profilePictureLink)
    {
        return Uri.TryCreate(profilePictureLink, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    protected override IEnumerable<Object> GetEqualityComponents()
    {
        yield return ProfilePictureLink;
    }
}



public static class ProfilePictureErrors
{
    public static readonly Error InvalidProfilePictureUrl = Error.Problem(
        "Users.InvalidProfilePictureUrl",
        "The provided profile picture URL is invalid. Please provide a valid URL.");
}