namespace Domain.Users.JoinTables;
using Domain.Users.Entities;
public class UserExpertise
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid ExpertiseId { get; set; }
    public Expertise Expertise { get; set; } = default!;
}

//public enum ProficiencyLevel
//{
//    Beginner ,
//    Intermediate,  
//    Advanced 
//}