using Application.Abstractions.Data;
using Domain.Users;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    // Users Modules : 
    public DbSet<User> Users { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    // join tables 
    public DbSet<UserLanguage> UserLanguages { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<MentorMentee> UserMentors { get; set; }

    // 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        // Get entities that implement IEntity
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IEntity)
            .Select(e => e.Entity as IEntity)
            .Where(e => e != null)
            .ToList();

        var allDomainEventsToDispatch = new List<IDomainEvent>();

        foreach (var entity in entitiesWithEvents)
        {
            var eventsFromThisEntity = entity.DomainEvents;

            if (eventsFromThisEntity.Any())
            {
                allDomainEventsToDispatch.AddRange(eventsFromThisEntity);
                entity.ClearDomainEvents();
            }
        }

        if (allDomainEventsToDispatch.Any())
        {
            await domainEventsDispatcher.DispatchAsync(allDomainEventsToDispatch, CancellationToken.None); // Consider passing the original cancellationToken
        }
    }
}
