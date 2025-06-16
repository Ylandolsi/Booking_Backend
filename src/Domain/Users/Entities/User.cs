using Domain.Users.JoinTables;
using Domain.Users.ValueObjects;
using Microsoft.AspNet.Identity.EntityFramework;
using SharedKernel;

namespace Domain.Users.Entities;

public sealed class User : IdentityUser
{
    public string PasswordHash { get; private set; } = default!;
    //public bool IsAdmin { get; init; } = false;
    public Status Status { get; private set; } = default!;
    public Name Name { get; private set; } = default!;

    public ProfilePicture ProfilePictureUrl { get; private set; } = default!;
    public EmailAdress EmailAddress { get; private set; } = default!;


    private User() { }


    public static Result<User> Create(
        string firstName,
        string lastName,
        string emailValue,
        string passwordHash,
        string profilePictureSource)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(UserErrors.PasswordCannotBeEmpty);
        }

        List<Error> validationErrors = new List<Error>();
        Name name = default!;
        try
        {
            name = new Name(firstName, lastName);
        }
        catch (ArgumentException ex)
        {
            validationErrors.Add(Error.Problem("User.InvalidName", ex.Message));
        }

        EmailAdress emailAddress = default!;
        try
        {
            emailAddress = new EmailAdress(emailValue);
        }
        catch (ArgumentException ex)
        {
            validationErrors.Add(Error.Problem("User.InvalidEmail", ex.Message));
        }

        ProfilePicture profilePicture = default!;
        try
        {
            profilePicture = new ProfilePicture(profilePictureSource);
        }
        catch (ArgumentException ex)
        {
            validationErrors.Add(Error.Problem("User.InvalidProfilePicture", ex.Message));
        }

        if (validationErrors.Any())
        {

            return Result.Failure<User>(new ValidationError(validationErrors.ToArray()!));
        }

        var user = new User
        {
            Name = name,
            PasswordHash = passwordHash,
            ProfilePictureUrl = profilePicture,
            Status = new Status(false),
            EmailAddress = emailAddress
        };


        return Result.Success(user);
    }


    public ICollection<Experience> Experiences { get; private set; } = new List<Experience>();
    public ICollection<Education> Educations { get; private set; } = new List<Education>();


    public ICollection<MentorMentee> UserMentors { get; private set; } = new List<MentorMentee>();

    public ICollection<MentorMentee> UserMentees { get; private set; } = new List<MentorMentee>();

    public ICollection<UserSkill> UserSkills { get; private set; } = new HashSet<UserSkill>();
    public ICollection<UserLanguage> UserLanguages { get; private set; } = new List<UserLanguage>();



    // Domain Events
    private DomainEventContainer _domainContainer = new DomainEventContainer();
    public List<IDomainEvent> DomainEvents => _domainContainer.DomainEvents;
    public void ClearDomainEvents() => _domainContainer.ClearDomainEvents();
    public void Raise(IDomainEvent domainEvent) => _domainContainer.Raise(domainEvent);


}
