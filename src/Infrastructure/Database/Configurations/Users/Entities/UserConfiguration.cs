using Domain.Users.Entities;
using Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Users.Entities;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // complex properties are stored in the same table as the entity
        // has one , create a separate table for the property
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Slug).IsUnique();

        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.FirstName).HasMaxLength(50).IsRequired();
            name.Property(n => n.LastName).HasMaxLength(50).IsRequired();
        });
        builder.OwnsOne(u => u.ProfilePictureUrl, profilePicture =>
        {
            profilePicture.Property(p => p.ProfilePictureLink)
                .HasMaxLength(256)
                .IsRequired();
        });


        builder.OwnsOne(u => u.Status, status => { });

        builder.OwnsOne(u => u.SocialLinks, social =>
        {
            social.Property(s => s.LinkedIn).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Twitter).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Github).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Youtube).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Facebook).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Instagram).HasMaxLength(256).IsRequired(false);
            social.Property(s => s.Portfolio).HasMaxLength(256).IsRequired(false);
        });

        builder.OwnsOne(u => u.ProfileCompletionStatus, profileCompletion =>
        {
            profileCompletion.Property(p => p.HasName);
            profileCompletion.Property(p => p.HasEmail);
            profileCompletion.Property(p => p.HasProfilePicture);
            profileCompletion.Property(p => p.HasBio);
            profileCompletion.Property(p => p.HasGender);
            profileCompletion.Property(p => p.HasSocialLinks);
            profileCompletion.Property(p => p.HasEducation);
            profileCompletion.Property(p => p.HasLanguages);
            profileCompletion.Property(p => p.HasExpertise);
        });

        builder.Property(u => u.Bio)
            .HasMaxLength(500)
            .IsRequired(false);
        
        builder.Property( u => u.Gender).HasMaxLength(10).IsRequired(false);




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

        // Many to many relationship 
        builder.HasMany(u => u.UserLanguages)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserExpertises)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
