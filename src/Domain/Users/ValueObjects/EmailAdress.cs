using System.Text.RegularExpressions; 
namespace Domain.Users.ValueObjects;

public class EmailAdress
{
    public string Email { get; private set; }

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
    }


}
