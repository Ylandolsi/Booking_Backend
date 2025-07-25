using SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Users.Entities;

public class Experience : Entity
{
    // TODO : Add industry to Experience 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public bool ToPresent { get; private set; }

    public int UserId { get; private set; }
    public User User { get; set; } = default!;


    private Experience() { }

    public Experience(string title,
                      string description,
                      string company,
                      int userId,
                      DateTime startDate,
                      DateTime? endDate = null)
    {

        Title = title?.Trim() ?? string.Empty;
        Description = description?.Trim() ?? string.Empty;
        StartDate = startDate;
        EndDate = endDate;
        Company = company?.Trim() ?? string.Empty;
        UserId = userId;
        ToPresent = !endDate.HasValue;
    }

    public void Update(string title,
                       string company,
                       DateTime startDate,
                       DateTime? endDate,
                       string? description)
    {
        Title = title.Trim();
        Company = company.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Description = description?.Trim() ?? string.Empty;
        ToPresent = !endDate.HasValue;

    }

    public Result Complete(DateTime endDate)
    {
        if (endDate < StartDate)
            return Result.Failure(ExperienceErrors.InvalidEndDate);

        EndDate = endDate;
        ToPresent = false;
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