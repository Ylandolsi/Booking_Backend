using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations.Users.Entities;

internal sealed class ExpertiseConfiguration : IEntityTypeConfiguration<Expertise>
{
    public void Configure(EntityTypeBuilder<Expertise> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasMany(s => s.UserExpertises)
            .WithOne(us => us.Expertise)
            .HasForeignKey(us => us.ExpertiseId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.Property(s => s.Category).
        //    HasConversion(
        //        v => v.ToString(),
        //        v => Enum.Parse<ExpertiseCategory>(v)
        //    );
    }


}
