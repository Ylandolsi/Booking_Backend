using Application.Abstractions.Data;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                IDomainEventsDispatcher omainEventsDispatcher) : base(options)
    {
        _domainEventsDispatcher = omainEventsDispatcher;

    }
    // Users Modules : 
    public DbSet<User> Users { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    // join tables 
    public DbSet<UserLanguage> UserLanguages { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<MentorMentee> UserMentors { get; set; }

    // 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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


    private async Task<List<IDomainEvent>> GetUsersDomainEvents()
    {
        var userEntitiesWithEvents = ChangeTracker
            .Entries<User>()
            .Select(entry => entry.Entity) // Get the User entity instances
            .Where(user => user.DomainEvents.Any()) // Filter for users that have domain events
            .ToList();

        List<IDomainEvent> userDomainEventsToDispatch = new List<IDomainEvent>();

        if (userEntitiesWithEvents.Any())
        {
            foreach (var userEntity in userEntitiesWithEvents)
            {

                userDomainEventsToDispatch.AddRange(userEntity.DomainEvents);
                userEntity.ClearDomainEvents();
            }
        }

        return userDomainEventsToDispatch;

    }


    private async Task PublishDomainEventsAsync()
    {
        // users events 
        var usersDomainEvents = await GetUsersDomainEvents();

        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        if (usersDomainEvents.Any())
        {
            domainEvents.AddRange(usersDomainEvents);
        }

        if (domainEvents.Any())
        {
            await _domainEventsDispatcher.DispatchAsync(domainEvents);
        }
    }
}
