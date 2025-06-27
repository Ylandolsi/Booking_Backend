using Domain.Users.JoinTables;
using SharedKernel;

namespace Domain.Users.Entities;

public class Skill : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Skill() { }

    public Skill(Guid id, string name, string description, SkillCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty", nameof(name));

        Id = id;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Category = category;
    }
    public SkillCategory Category { get; private set; }
    public ICollection<UserSkill> UserSkills { get; private set; } = new List<UserSkill>();


}

public enum SkillCategory
{
    Technical,
    SoftSkil
}