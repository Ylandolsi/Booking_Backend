using System.Text.RegularExpressions;
using SharedKernel;

namespace Domain.Users.ValueObjects;

public class EmailAdress : ValueObject
{
    public string Email { get; private set; }
    public bool Verified { get; private set; } = false;

    public EmailAdress(string email)
    {
        if (email is null || email.Trim().Length == 0)
        {
            throw new ArgumentException("Email address cannot be null or empty.", nameof(email));
        }
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email address format.", nameof(email));
        }

        Email = email;
        Verified = false;

    }
    public bool IsVerified()
    {
        return Verified;
    }
    public void Verify()
    {
        Verified = true;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return Verified;
    }
    public override string ToString() => $"Email: {Email}, Verified: {Verified}";





}
