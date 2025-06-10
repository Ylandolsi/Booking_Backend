
using SharedKernel; 

namespace Domain.Users.Entities;

public class RefreshToken : Entity
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsRevoked => RevokedOnUtc.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public RefreshToken(string token, Guid userId, DateTime expiresOnUtc)
    {
        Id = Guid.NewGuid();
        Token = token ?? throw new ArgumentNullException(nameof(token));
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (!IsActive)
        {
            return;
        }
        RevokedOnUtc = DateTime.UtcNow;
    }
}
