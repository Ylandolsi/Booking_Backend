namespace Domain.Users.JoinTables;

using Domain.Users.Entities;
public class UserExpertise
{
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int ExpertiseId { get; set; }
    public Expertise Expertise { get; set; } = default!;
}
