using System.ComponentModel.DataAnnotations;
using Domain.Users.JoinTables;
using Microsoft.AspNetCore.Identity;
using Domain.Users.ValueObjects;
using SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Users.Entities;

public static class Genders
{
    public const string Male = "Male";
    public const string Female = "Female";

    public static readonly HashSet<string> ValidGenders = new() { Male, Female };
    public static bool IsValid(string gender) => ValidGenders.Contains(gender);
}

public sealed class User : IdentityUser<int>, IEntity
{

    public string Slug { get; private set; } = string.Empty;
    public Name Name { get; private set; } = default!;
    public Status Status { get; private set; } = default!;
    public ProfilePicture ProfilePictureUrl { get; private set; } = default!;
    public string Gender { get; private set; } = "Male";
    public SocialLinks SocialLinks { get; private set; } = default!;
    public ProfileCompletionStatus ProfileCompletionStatus { get; private set; } = new ProfileCompletionStatus();

    public string Bio { get; private set; } = string.Empty;


    private User() { }


    public static User Create(
        string slug,
        string firstName,
        string lastName,
        string emailAddress,
        string profilePictureSource)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be null or empty", nameof(slug));

        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email cannot be null or empty", nameof(emailAddress));

        var user = new User
        {
            Name = new Name(firstName, lastName),
            Status = new Status(false),
            Email = emailAddress,
            UserName = emailAddress,
            ProfilePictureUrl = new ProfilePicture(profilePictureSource),
            Slug = slug,
        };

        return user;
    }

    public void UpdateSocialLinks(SocialLinks links)
    {
        SocialLinks = links;
    }
    public void UpdateBio(string bio)
    {
        if (bio?.Length > UserConstraints.MaxBioLength)
            throw new ArgumentException($"Bio cannot exceed {UserConstraints.MaxBioLength} characters");
        Bio = bio?.Trim() ?? string.Empty;
    }

    public void UpdateGender(string gender)
    {
        if (!Genders.IsValid(gender))
            throw new ArgumentException("Invalid gender");
        Gender = gender;
    }

    public void UpdateName(string firstName, string lastName)
    {
        Name = new Name(firstName, lastName);
    }

    // TODO : configure these one to many as readonly 
    //builder.Navigation(o => o.Items)
    // .UsePropertyAccessMode(PropertyAccessMode.Field);

    // ONE TO MANY 
    public ICollection<Experience> Experiences { get; private set; } = new List<Experience>();
    public ICollection<Education> Educations { get; private set; } = new List<Education>();


    // MANY TO MANY RELATIONSHIP

    public ICollection<MentorMentee> UserMentors { get; private set; } = new List<MentorMentee>();

    public ICollection<MentorMentee> UserMentees { get; private set; } = new List<MentorMentee>();
    // MAX 4
    public ICollection<UserExpertise> UserExpertises { get; private set; } = new HashSet<UserExpertise>();
    // MAX 4 
    public ICollection<UserLanguage> UserLanguages { get; private set; } = new List<UserLanguage>();



    // Domain Events
    private DomainEventContainer _domainContainer = new DomainEventContainer();

    [NotMapped] // for ef core 
    [JsonIgnore] // even though it wont get mapped , we need to ignore it for serialization
    public List<IDomainEvent> DomainEvents => _domainContainer.DomainEvents;
    public void ClearDomainEvents() => _domainContainer.ClearDomainEvents();
    public void Raise(IDomainEvent domainEvent) => _domainContainer.Raise(domainEvent);


}
