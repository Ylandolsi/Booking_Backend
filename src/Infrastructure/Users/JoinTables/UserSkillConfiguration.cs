using Domain.Users.JoinTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users.JoinTables;

internal sealed class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.HasKey(ul => new { ul.UserId, ul.SkillId });

        builder.HasOne(ul => ul.User)
            .WithMany(u => u.UserSkills)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ul => ul.Skill)
            .WithMany(s => s.UserSkills)
            .HasForeignKey(ul => ul.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}