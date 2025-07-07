using Domain.Users.JoinTables;
using SharedKernel;

namespace Domain.Users.Entities;

public class Expertise : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Expertise() { }

    public Expertise(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Expertise name cannot be empty", nameof(name));

        Id = id;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        //Category = category;
    }
    public ICollection<UserExpertise> UserExpertises { get; private set; } = new List<UserExpertise>();
    //public ExpertiseCategory Category { get; private set; }


}

//public enum ExpertiseCategory
//{
//    Technical,
//    SoftSkil
//}