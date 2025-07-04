﻿using Domain.Users.JoinTables;
using Microsoft.AspNetCore.Identity;
using Domain.Users.ValueObjects;
using SharedKernel;

namespace Domain.Users.Entities;

public sealed class User : IdentityUser<Guid>, IEntity
{
    public Name Name { get; private set; } = default!;
    public Status Status { get; private set; } = default!;
    public ProfilePicture ProfilePictureUrl { get; private set; } = default!;


    private User() { }


    public static User Create(
        string firstName,
        string lastName,
        string emailAddress,
        string profilePictureSource)
    {
        var user = new User
        {
            Name = new Name(firstName, lastName),
            Status = new Status(false),
            Email = emailAddress,
            UserName = emailAddress,
            ProfilePictureUrl = new ProfilePicture(profilePictureSource)
        };

        return user;
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
