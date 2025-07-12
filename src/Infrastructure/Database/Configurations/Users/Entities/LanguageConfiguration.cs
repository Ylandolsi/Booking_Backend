using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Database.Configurations.Users.Entities;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(l => l.UserLanguages)
            .WithOne(ul => ul.Language)
            .HasForeignKey(ul => ul.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
