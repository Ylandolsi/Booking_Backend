using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Users.JoinTables;
using SharedKernel;

namespace Domain.Users.Entities;

public class Expertise : Entity
{
    // add default color ? 
    // bg-orange-100 text-orange-600 || bg-green-100 text-green-600
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Expertise() { }

    public Expertise(string name, string description, int id = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Expertise name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        if (id != 0)
            Id = id;
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