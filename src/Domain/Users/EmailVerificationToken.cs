using Domain.Users.Entities;
using SharedKernel;

namespace Domain.Users;

public class EmailVerificationToken : Entity
{
    private const int TokenExpirationMinutes = 5;

    public Guid Id { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime ExpiresOnUtc { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;


    public EmailVerificationToken(Guid userId)
    {
        Id = Guid.NewGuid();
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