using Domain.Users.ValueObjects;

namespace Application.Users.Authentication.Utils;

// mentor and mentee retrieve them manually 
public record UserResponse(
    string Slug,
    string FirstName,
    string LastName,
    Status Status,
    ProfilePicture ProfilePicture,
    string Gender,
    SocialLinks SocialLinks,
    string Bio,
    List<Domain.Users.Entities.Experience> Experiences,
    List<Domain.Users.Entities.Education> Educations,
    List<Domain.Users.Entities.Expertise> Expertises,
    List<Domain.Users.Entities.Language> Languages);