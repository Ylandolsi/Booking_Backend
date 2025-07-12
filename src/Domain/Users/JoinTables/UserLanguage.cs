using Domain.Users.Entities;

namespace Domain.Users.JoinTables;

public class UserLanguage
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public int LanguageId { get; set; }
    public Language Language { get; set; } = default!;
}
