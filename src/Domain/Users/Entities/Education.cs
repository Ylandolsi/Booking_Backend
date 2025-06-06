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
    
    public static Result<Education> Create(
        string title,
        string description,
        DateTime startDate,
        string university,
        Guid userId,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Education>(Error.Problem("Education.InvalidTitle", "Title cannot be empty"));

        if (string.IsNullOrWhiteSpace(university))
            return Result.Failure<Education>(Error.Problem("Education.InvalidUniversity", "University cannot be empty"));

        if (endDate.HasValue && endDate < startDate)
            return Result.Failure<Education>(Error.Problem("Education.InvalidEndDate", "End date cannot be before start date"));

        var education = new Education(title, description, startDate, university, userId, endDate);
        return Result.Success(education);
    }

    public Education(string title, string description, DateTime startDate,
                    string university, Guid userId, DateTime? endDate = null)
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