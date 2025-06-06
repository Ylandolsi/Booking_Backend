using SharedKernel;
using Domain.Users.JoinTables;
namespace Domain.Users.Entities;

public class Language : Entity 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<UserLanguage> UserLanguages { get; private set; } = new List<UserLanguage>();

    private Language() { }
    public Language(string name)
    {
        Name = name?.Trim() ?? string.Empty;
    }
    public static  Result<Language> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Language>(Error.Problem("Language.InvalidName", "Language name cannot be empty"));

        return new Language(name);
    }
}