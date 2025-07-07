using SharedKernel;

namespace Domain.Users.ValueObjects;

public class SocialLinks : ValueObject
{
    public string? LinkedIn { get; private set; }
    public string? Twitter { get; private set; }
    public string? Github { get; private set; }

    public string? Youtube { get; private set; }
    public string? Facebook { get; private set; }
    public string? Instagram { get; private set; }
    public string? Portfolio { get; private set; }
    public SocialLinks(string? linkedIn = null,
                       string? twitter = null,
                       string? github = null,
                       string? youtube = null,
                       string? facebook = null,
                       string? instagram = null,
                       string? portfolio = null)
    {
        LinkedIn = linkedIn;
        Twitter = twitter;
        Github = github;
        Youtube = youtube;
        Facebook = facebook;
        Instagram = instagram;
        Portfolio = portfolio;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return LinkedIn ?? "";
        yield return Twitter ?? "";
        yield return Github ?? "";
        yield return Youtube ?? "";
        yield return Facebook ?? "";
        yield return Instagram ?? "";
        yield return Portfolio ?? "";
    }
}