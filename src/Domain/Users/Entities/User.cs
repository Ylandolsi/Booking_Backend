using Domain.Users.JoinTables;
using Domain.Users.ValueObjects;
using SharedKernel;

namespace Domain.Users.Entities;

public sealed class User : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Name Name { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Status Status { get; set; } = default!;

    public ProfilePicture ProfilePictureUrl { get; private set; } = default!;
    public EmailAdress EmailAddress { get; private set; } = default!;

    private User() { }


    public static Result<User> Create(
        Guid id,
        string firstName,
        string lastName,
        string emailValue,
        string passwordHash,
        string profilePictureSource,
        bool isMentor = false)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(UserErrors.PasswordCannotBeEmpty);
        }

        Name name;
        try
        {
            name = new Name(firstName, lastName);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<User>(Error.Problem("User.InvalidName", ex.Message));
        }

        EmailAdress emailAddress;
        try
        {
            emailAddress = new EmailAdress(emailValue);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<User>(Error.Problem("User.InvalidEmail", ex.Message));
        }

        ProfilePicture profilePicture;
        try
        {
            profilePicture = new ProfilePicture(profilePictureSource);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<User>(Error.Problem("User.InvalidProfilePicture", ex.Message));
        }

        var user = new User
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Name = name,
            PasswordHash = passwordHash,
            ProfilePictureUrl = profilePicture,
            Status = new Status(isMentor),
            EmailAddress = emailAddress
        };



        // user.Raise(new UserRegisteredDomainEvent(user.Id));

        return Result.Success(user);
    }


    public ICollection<Experience> Experiences { get; private set; } = new List<Experience>();
    public ICollection<Education> Educations { get; private set; } = new List<Education>();


    public ICollection<MentorMentee> UserMentors { get; private set; } = new List<MentorMentee>();

    public ICollection<MentorMentee> UserMentees { get; private set; } = new List<MentorMentee>();

    public ICollection<UserSkill> UserSkills { get; private set; } = new HashSet<UserSkill>();
    public ICollection<UserLanguage> UserLanguages { get; private set; } = new List<UserLanguage>();
    


}
