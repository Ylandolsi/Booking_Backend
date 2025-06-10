using Domain.Users.Entities;
using Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users.Entities;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.ComplexProperty(u => u.Name, name =>
        {
            name.Property(n => n.FirstName).HasMaxLength(50).IsRequired();
            name.Property(n => n.LastName).HasMaxLength(50).IsRequired();

        });

        builder.ComplexProperty(u => u.EmailAddress, email =>
        {
            email.Property(e => e.Email)
                .HasMaxLength(256)
                .IsRequired();
        });

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.ComplexProperty(u => u.ProfilePictureUrl, profilePicture =>
        {
            profilePicture.Property(p => p.ProfilePictureLink)
                .HasMaxLength(256)
                .IsRequired();
        });

        builder.ComplexProperty(u => u.Status, status => { }); 


        builder.HasMany(u => u.UserLanguages)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserSkills)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // List of mentors that the user is dealt with 
        builder.HasMany(u => u.UserMentors)
            .WithOne(um => um.Mentee)
            .HasForeignKey(um => um.MenteeId)
            .OnDelete(DeleteBehavior.Cascade);

        // List of users that the user is mentoring
        builder.HasMany(u => u.UserMentees)
            .WithOne(um => um.Mentor)
            .HasForeignKey(um => um.MentorId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(u => u.Experiences)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(u => u.Educations)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
