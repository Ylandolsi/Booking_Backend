namespace Domain.Users.JoinTables;
using Domain.Users.Entities;
public class MentorMentee
{
    
    public Guid MentorId { get; set; }
    public User Mentor { get; set; } = default!;

    public Guid MenteeId { get; set; }
    public User Mentee { get; set; } = default!;

    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;

}
