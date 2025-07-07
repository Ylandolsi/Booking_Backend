using SharedKernel;
namespace Domain.Users.ValueObjects;

public class Name : ValueObject
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    private Name() { }

    public Name(string firstName, string? lastName)
    {

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        FirstName = firstName.Trim();
        LastName = lastName?.Trim() ?? "";
    }

    public string FullName => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}