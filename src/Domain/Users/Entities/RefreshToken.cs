
using SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Users.Entities;

public class RefreshToken : Entity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public string Token { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsRevoked => RevokedOnUtc.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public string CreatedByIp { get; private set; }
    public string UserAgent { get; private set; }

    private RefreshToken() { }

    public RefreshToken(string token,
                        int userId,
                        DateTime expiresOnUtc,
                        string createdByIp,
                        string userAgent)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
        CreatedOnUtc = DateTime.UtcNow;
        // TODO : uncomment the exceptions
        // the exception are removed for testing purposes ( cuz in memory there is no IP or User-Agent)
        CreatedByIp = createdByIp ?? string.Empty; //?? throw new ArgumentNullException(nameof(createdByIp));
        UserAgent = userAgent ?? string.Empty; //  ?? throw new ArgumentNullException(nameof(userAgent));
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
