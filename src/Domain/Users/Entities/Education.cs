using SharedKernel;

namespace Domain.Users.Entities;

public class Education : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string University { get; private set; } = string.Empty;
    public bool IsCurrent { get; private set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    private Education() { }
    

    public Education(string title,
                     string description,
                     DateTime startDate,
                     string university,
                     Guid userId,
                     DateTime? endDate = null)
    {

        Title = title?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        StartDate = startDate;
        EndDate = endDate;
        University = university?.Trim() ?? string.Empty;
        UserId = userId;
        IsCurrent = !endDate.HasValue;
    }

    public Result Complete(DateTime endDate)
    {
        if (endDate < StartDate)
            return Result.Failure(Error.Problem("Education.InvalidEndDate", "End date cannot be before start date"));

        EndDate = endDate;
        IsCurrent = false;
        return Result.Success();
    }


}