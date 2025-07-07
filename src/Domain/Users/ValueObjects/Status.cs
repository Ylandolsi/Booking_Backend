using SharedKernel;

namespace Domain.Users.ValueObjects;

public class Status : ValueObject
{
    public bool IsMentor { get; private set; }
    public bool IsActive { get; private set; }

    private Status() { }

    public Status(bool isMentor)
    {

        IsMentor = isMentor;
        IsActive = isMentor;
    }

    public void BecomeMentor()
    {
        IsMentor = true;
    }


    public Result ToggleActivate()
    {
        if (!IsMentor)
        {
            throw new InvalidOperationException("Only mentors can be activated or deactivated.");
        }
        IsActive = !IsActive;
        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsMentor;
        yield return IsActive;
    }

    public override string ToString()
    {
        return $"Mentor: {IsMentor}, Active: {IsActive}";
    }
}