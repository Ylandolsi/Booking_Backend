using SharedKernel;

namespace Domain.Users.Entities;

public class Experience : Entity
{
    // TODO : Add industry to Experience 
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
    

    public Experience(string title,
                      string description,
                      string companyName,
                      Guid userId,
                      DateTime startDate,
                      DateTime? endDate = null)
    {
   
        Title = title?.Trim() ?? string.Empty ;
        Description = description?.Trim() ?? string.Empty;
        StartDate = startDate;
        EndDate = endDate;
        CompanyName = companyName?.Trim() ?? string.Empty ;
        UserId = userId;
        IsCurrent = !endDate.HasValue;
    }

    public void Update(string title,
                       string companyName,
                       DateTime startDate,
                       DateTime? endDate,
                       string? description)
    {
        Title = title.Trim();
        CompanyName = companyName.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Description = description?.Trim() ?? string.Empty;
        IsCurrent = !endDate.HasValue;
        
    }

    public Result Complete(DateTime endDate)
    {
        if (endDate < StartDate)
            return Result.Failure(ExperienceErrors.InvalidEndDate);

        EndDate = endDate;
        IsCurrent = false;
        return Result.Success();
    }


}

public static class ExperienceErrors
{
    public static readonly Error InvalidTitle = Error.Problem("Experience.InvalidTitle", "Title cannot be empty");
    public static readonly Error InvalidCompanyName = Error.Problem("Experience.InvalidCompanyName", "Company name cannot be empty");
    public static readonly Error InvalidEndDate = Error.Problem("Experience.InvalidEndDate", "End date cannot be before start date");
    public static readonly Error ExperienceNotFound = Error.NotFound("Experience.NotFound", "Experience not found");
}