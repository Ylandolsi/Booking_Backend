using SharedKernel;

namespace Domain.Users.Entities;

public class Experience : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public bool IsCurrent { get; private set; }
    
    public Guid UserId { get; private set; }
    public User User { get; set; } = default!;

    private Experience() { }
    
    public static Result<Experience> Create(
        string title,
        string description,
        DateTime startDate,
        string companyName,
        Guid userId,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Experience>(Error.Problem("Experience.InvalidTitle", "Title cannot be empty"));

        if (string.IsNullOrWhiteSpace(companyName))
            return Result.Failure<Experience>(Error.Problem("Experience.InvalidCompanyName", "Company name cannot be empty"));

        if (endDate.HasValue && endDate < startDate)
            return Result.Failure<Experience>(Error.Problem("Experience.InvalidEndDate", "End date cannot be before start date"));

        var experience = new Experience(title, description, companyName, userId,startDate ,  endDate);
        return Result.Success(experience);
    }
    public Experience( string title , string description,
                     string companyName, Guid userId, DateTime startDate ,  DateTime? endDate = null)
    {
    

        Title = title?.Trim() ?? string.Empty ;
        Description = description?.Trim() ?? string.Empty;
        StartDate = startDate;
        EndDate = endDate;
        CompanyName = companyName?.Trim() ?? string.Empty ;
        UserId = userId;
        IsCurrent = !endDate.HasValue;
    }

    public Result Complete(DateTime endDate)
    {
        if (endDate < StartDate)
            return Result.Failure(Error.Problem("Experience.InvalidEndDate", "End date cannot be before start date"));

        EndDate = endDate;
        IsCurrent = false;
        return Result.Success();
    }
}