namespace Domain.Users.JoinTables;
using Domain.Users.Entities;
public class UserSkill
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = default!;

    public ProficiencyLevel ProficiencyLevel { get; set; }
}

public enum ProficiencyLevel
{
    Beginner ,
    Intermediate,  
    Advanced 
}