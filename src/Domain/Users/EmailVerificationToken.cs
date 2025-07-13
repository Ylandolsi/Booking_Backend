using Domain.Users.Entities;
using SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Users;

public class EmailVerificationToken : Entity
{
    private const int TokenExpirationMinutes = 5;
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime ExpiresOnUtc { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = default!;


    public EmailVerificationToken(int userId)
    {
        UserId = userId;
        CreatedOnUtc = DateTime.UtcNow;
        ExpiresOnUtc = DateTime.UtcNow.AddMinutes(TokenExpirationMinutes);

    }
    public void UpdateExpiration()
    {
        CreatedOnUtc = DateTime.UtcNow;
        ExpiresOnUtc = DateTime.UtcNow.AddMinutes(TokenExpirationMinutes);
    }


    public bool IsStillValid() => DateTime.UtcNow < ExpiresOnUtc ;
    
    
    
}