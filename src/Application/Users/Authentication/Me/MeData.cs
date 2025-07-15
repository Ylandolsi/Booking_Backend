using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Domain.Users.ValueObjects;

namespace Application.Users.Authentication.Me;

public record MeData(
    string Slug,
    string? FirstName,
    string? LastName,
    Status Status,
    ProfilePicture ProfilePicture,
    string Gender,
    SocialLinks SocialLinks,
    string Bio,
    List<Domain.Users.Entities.Experience> Experiences,
    List<Domain.Users.Entities.Education> Educations,
    List<Domain.Users.Entities.Expertise> Expertises,
    List<Domain.Users.Entities.Language> Languages,
    int ProfileCompletionStatus);
    