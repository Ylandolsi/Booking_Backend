using Domain.Users.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Users.JoinTables;

internal sealed class UserExpertiseConfiguration : IEntityTypeConfiguration<UserExpertise>
{
    public void Configure(EntityTypeBuilder<UserExpertise> builder)
    {
        builder.HasKey(ul => new { ul.UserId, ul.ExpertiseId });

        builder.HasOne(ul => ul.User)
            .WithMany(u => u.UserExpertises)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ul => ul.Expertise)
            .WithMany(s => s.UserExpertises)
            .HasForeignKey(ul => ul.ExpertiseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}