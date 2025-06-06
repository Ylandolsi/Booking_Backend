using SharedKernel;

namespace Domain.Users.ValueObjects;

public class ProfilePicture
{
    const string DefaultProfilePictureUrl = "https://www.pngarts.com/files/10/Default-Profile-Picture-PNG-Download-Image.png";
    public string ProfilePictureLink { get; private set; }

    public ProfilePicture(string pLink = "")
    
    {
        if (string.IsNullOrWhiteSpace(pLink))
        {
            ProfilePictureLink = DefaultProfilePictureUrl;
            return;
        }

        if (!IsValidUrl(pLink) )
        {
            throw new ArgumentException("Invalid profile picture URL", nameof(pLink) );
        }

        ProfilePictureLink = pLink ;
    }


    public Result UpdateProfilePicture(string pLink)
    {
        if (string.IsNullOrEmpty(pLink))
        {
            return Result.Failure(ProfilePictureErrors.InvalidProfilePictureUrl);
        }
        if (!IsValidUrl(pLink))
        {
            return Result.Failure(ProfilePictureErrors.InvalidProfilePictureUrl);
        }

        ProfilePictureLink = pLink;
        return Result.Success();
    }
    

    public Result ResetToDefaultProfilePicture()
    {
        ProfilePictureLink = DefaultProfilePictureUrl;
        return Result.Success();
    }

    private static bool IsValidUrl(string pLink)
    {
        return Uri.TryCreate(pLink, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

}



public static class ProfilePictureErrors
{
    public static readonly Error InvalidProfilePictureUrl = Error.Problem(
        "Users.InvalidProfilePictureUrl",
        "The provided profile picture URL is invalid. Please provide a valid URL.");
}