using SharedKernel;
using Domain.Users.JoinTables;
namespace Domain.Users.Entities;

public class Language : Entity
{
    // it will be hardc
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<UserLanguage> UserLanguages { get; private set; } = new List<UserLanguage>();

    private Language() { }
    public Language(string name)
    {
        Name = name?.Trim() ??
                    throw new ArgumentException("name should not be empty or null");
    }


}