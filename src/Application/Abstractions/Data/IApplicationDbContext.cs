using Domain.Users;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    // Users Modules : 
    public DbSet<User> Users { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Expertise> Expertises { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    // join tables 
    public DbSet<UserLanguage> UserLanguages { get; set; }
    public DbSet<UserExpertise> UserExpertises { get; set; }
    public DbSet<MentorMentee> UserMentors { get; set; }

    // outbox messages
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
