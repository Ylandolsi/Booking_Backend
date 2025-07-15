using Application.Abstractions.Data;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Infrastructure.Database.Seed.Users;
using Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext
    : IdentityDbContext<User, IdentityRole<int>, int>, IApplicationDbContext
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


    // 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // to avoid the warning about DateTime && GUID for seeding data  
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ;

        modelBuilder.HasDefaultSchema(Schemas.Default);
        SeedData.Seed(modelBuilder);
    }
    // protected override void Up(MigrationBuilder migrationBuilder)
    // {
    // benefit : migration depends only on database schemas ( dont get affected when the model get changed (adding new require property wont break the migration ))
    //     migrationBuilder.SqlFile("InsertLanguages.sql", true);
    //     migrationBuilder.SqlFile("InsertExpertise.sql", true);
    // }



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ConvertDomainEventsToOutboxMessages();
        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task ConvertDomainEventsToOutboxMessages()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> events = entity.DomainEvents;
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        var userDomainEvents = ChangeTracker
            .Entries<User>()
            .Select(entry => entry.Entity)
            .SelectMany(user =>
            {
                List<IDomainEvent> events = user.DomainEvents;
                user.ClearDomainEvents();
                return events;
            })
            .ToList();

        domainEvents.AddRange(userDomainEvents);


        // TODO : check this approach instead of jsonConvert : 
        // var typeInfo = new Dictionary<string, string>
        // {
        //     ["Type"] = domainEvent.GetType().FullName!
        // };
        // var serializedData = JsonConvert.SerializeObject(domainEvent);
        // var metadata = JsonConvert.SerializeObject(typeInfo);

        // Store type info separately
        // var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage(
        // domainEvent.GetType().AssemblyQualifiedName!,
        // JsonConvert.SerializeObject(domainEvent),
        // DateTime.UtcNow
        // )).ToList();

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage(
            domainEvent.GetType().FullName!,
            JsonConvert.SerializeObject(
                domainEvent,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }),
            DateTime.UtcNow
        )).ToList();

        await OutboxMessages.AddRangeAsync(outboxMessages);
    }

}
