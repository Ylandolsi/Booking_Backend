using Application.Abstractions.Data;
using Domain.Users;
using Domain.Users.Entities;
using Domain.Users.JoinTables;
using Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IntegrationsTests.Seed;

public class TestProfileSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new();

    public TestProfileSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<List<User>> SeedComprehensiveUserProfilesAsync()
    {
        // Create the 3 diverse user profiles
        var users = new List<User>
        {
            await CreateSoftwareEngineerProfileAsync(),
            await CreateMarketingConsultantProfileAsync(),
            await CreateFinanceAdvisorProfileAsync()
        };

        return users;
    }

    private async Task<User> CreateSoftwareEngineerProfileAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Create base user
        var user = User.Create(
            "john-doe",
            "John",
            "Doe",
            "john.doe@example.com",
            "https://randomuser.me/api/portraits/men/32.jpg"
        );

        // Set password and create user
        var result = await userManager.CreateAsync(user, "Password123!");
        if (result.Succeeded)
            await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));

        // Update additional properties
        user.UpdateBio("Full-stack developer with 8+ years of experience in web and mobile development. Specialized in React, .NET Core, and cloud architecture.");
        user.UpdateGender(Genders.Male);
        user.UpdateSocialLinks(new SocialLinks(
            "https://github.com/johndoe",
            "https://linkedin.com/in/johndoe",
            "https://twitter.com/johndoe"
        ));

        // Add expertise (4 items max)
        var expertises = await context.Expertises.Take(4).ToListAsync();
        foreach (var expertise in expertises)
        {
            await context.UserExpertises.AddAsync(new UserExpertise
            {
                UserId = user.Id,
                ExpertiseId = expertise.Id
            });
        }

        // Add languages (4 items max)
        var languages = await context.Languages.Take(3).ToListAsync();
        foreach (var language in languages)
        {
            await context.UserLanguages.AddAsync(new UserLanguage
            {
                UserId = user.Id,
                LanguageId = language.Id,
            });
        }

        // Add experience
        await context.Experiences.AddRangeAsync(new List<Experience>
        {
            new Experience(
                title: "Senior Software Engineer",
                description: "Leading development of cloud-native applications using microservices architecture.",
                userId: user.Id,
                company: "Tech Solutions Inc.",
                startDate: new DateTime(2018, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                endDate: null
            ),
            new Experience(
                title: "Software Developer",
                description: "Developed and maintained web applications using React and ASP.NET Core.",
                userId: user.Id,
                company: "Digital Innovations",
                startDate: new DateTime(2015, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2018, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            )
        });

        // Add education
        await context.Educations.AddRangeAsync(new List<Education>
        {
            new Education(
                field: "Computer Science",
                description: "Specialized in Artificial Intelligence and Machine Learning",
                university: "University of Technology",
                userId: user.Id,
                startDate: new DateTime(2013, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2015, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            ),
            new Education(
                field: "Computer Science",
                description: "Focus on Software Engineering",
                university: "State University",
                userId: user.Id,
                startDate: new DateTime(2009, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2013, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            )
        });

        await context.SaveChangesAsync();
        return user;
    }

    private async Task<User> CreateMarketingConsultantProfileAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Create base user
        var user = User.Create(
            "jane-smith",
            "Jane",
            "Smith",
            "jane.smith@example.com",
            "https://randomuser.me/api/portraits/women/44.jpg"
        );

        // Set password and create user
        var result = await userManager.CreateAsync(user, "Password123!");
        if (result.Succeeded)
            await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));

        // Update additional properties
        user.UpdateBio("Marketing strategist with focus on digital transformation and brand development. Experienced in helping startups establish market presence.");
        user.UpdateGender(Genders.Female);
        user.UpdateSocialLinks(new SocialLinks(
            null,
            "https://linkedin.com/in/janesmith",
            "https://twitter.com/janesmith"
        ));

        // Add expertise (different set from first user)
        var expertises = await context.Expertises.Skip(4).Take(3).ToListAsync();
        foreach (var expertise in expertises)
        {
            await context.UserExpertises.AddAsync(new UserExpertise
            {
                UserId = user.Id,
                ExpertiseId = expertise.Id
            });
        }

        // Add languages (different set)
        var languages = await context.Languages.Skip(3).Take(4).ToListAsync();
        foreach (var language in languages)
        {
            await context.UserLanguages.AddAsync(new UserLanguage
            {
                UserId = user.Id,
                LanguageId = language.Id,
            });
        }

        // Add experience
        await context.Experiences.AddRangeAsync(new List<Experience>
        {
            new Experience(
                userId: user.Id,
                company: "Marketing Wizards",
                title: "Senior Marketing Consultant",
                startDate: new DateTime(2019, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: null, // Current job
                description: "Providing strategic marketing consultation to tech startups and established businesses."
            ),
            new Experience(
                userId: user.Id,
                company: "Global Brands Inc.",
                title: "Marketing Manager",
                startDate: new DateTime(2016, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2019, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                description: "Managed marketing campaigns for Fortune 500 clients across multiple industries."
            ),
            new Experience(
                userId: user.Id,
                company: "Creative Solutions Agency",
                title: "Marketing Associate",
                startDate: new DateTime(2014, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2016, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                description: "Developed and executed social media strategies for small businesses."
            )
        });

        // Add education
        await context.Educations.AddRangeAsync(new List<Education>
        {
            new Education(
                field: "Marketing",
                description: "Focus on Digital Marketing and Brand Development",
                university: "Business School of Excellence",
                userId: user.Id,
                startDate: new DateTime(2012, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2014, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            ),
            new Education(
                field: "Business Administration",
                description: "Marketing and Communications",
                university: "State University",
                userId: user.Id,
                startDate: new DateTime(2008, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2012, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            )
        });

        await context.SaveChangesAsync();
        return user;
    }

    private async Task<User> CreateFinanceAdvisorProfileAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Create base user
        var user = User.Create(
            "michael-johnson",
            "Michael",
            "Johnson",
            "michael.johnson@example.com",
            "https://randomuser.me/api/portraits/men/67.jpg"
        );

        // Set password and create user
        var result = await userManager.CreateAsync(user, "Password123!");
        if (result.Succeeded)
            await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));

        // Update additional properties
        user.UpdateBio("Certified financial advisor with 15+ years of experience. Specializing in investment strategies, retirement planning, and wealth management for high net worth individuals.");
        user.UpdateGender(Genders.Male);
        user.UpdateSocialLinks(new SocialLinks(
            null,
            "https://linkedin.com/in/michaeljohnson",
            null
        ));

        // Add expertise (different set from others)
        var expertises = await context.Expertises.Skip(7).Take(2).ToListAsync();
        foreach (var expertise in expertises)
        {
            await context.UserExpertises.AddAsync(new UserExpertise
            {
                UserId = user.Id,
                ExpertiseId = expertise.Id
            });
        }

        // Add languages (different set)
        var languages = await context.Languages.Skip(7).Take(2).ToListAsync();
        foreach (var language in languages)
        {
            await context.UserLanguages.AddAsync(new UserLanguage
            {
                UserId = user.Id,
                LanguageId = language.Id,
            });
        }

        // Add experience
        await context.Experiences.AddRangeAsync(new List<Experience>
        {
            new Experience(
                title: "Senior Financial Consultant",
                description: "Providing comprehensive financial planning services to high net worth individuals.",
                userId: user.Id,
                company: "Wealth Advisors Group",
                startDate: new DateTime(2015, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: null
            ),
            new Experience(
                title: "Financial Advisor",
                description: "Managed investment portfolios for clients with assets exceeding $10M.",
                userId: user.Id,
                company: "Investment Partners LLC",
                startDate: new DateTime(2010, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2015, 7, 15, 0, 0, 0, DateTimeKind.Utc)
            ),
            new Experience(
                title: "Junior Financial Analyst",
                description: "Analyzed investment opportunities and market trends for the wealth management division.",
                userId: user.Id,
                company: "Global Bank",
                startDate: new DateTime(2007, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            )
        });

        // Add education
        await context.Educations.AddRangeAsync(new List<Education>
        {
            new Education(
                field: "Finance",
                description: "Professional certification in investment analysis and portfolio management",
                university: "Finance Institute",
                userId: user.Id,
                startDate: new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2010, 12, 31, 0, 0, 0, DateTimeKind.Utc)
            ),
            new Education(
                field: "Finance",
                description: "Specialization in Investment Management",
                university: "Economic University",
                userId: user.Id,
                startDate: new DateTime(2005, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2007, 5, 30, 0, 0, 0, DateTimeKind.Utc)
            ),
            new Education(
                field: "Economics",
                description: "Focus on Financial Economics",
                university: "State University",
                userId: user.Id,
                startDate: new DateTime(2001, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                endDate: new DateTime(2005, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            )
        });

        // Set up mentorship relationships between users
        // First user is a mentor to second user
        if (await context.Users.CountAsync() >= 2)
        {
            var firstUser = await context.Users.FirstAsync(u => u.Email == "john.doe@example.com");
            await context.UserMentors.AddAsync(new MentorMentee
            {
                MentorId = firstUser.Id,
                MenteeId = user.Id
            });
        }

        await context.SaveChangesAsync();
        return user;
    }

    // Helper method for setting up mentor relationships
    public async Task CreateMentorshipRelationshipsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var john = await context.Users.FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");
        var jane = await context.Users.FirstOrDefaultAsync(u => u.Email == "jane.smith@example.com");
        var michael = await context.Users.FirstOrDefaultAsync(u => u.Email == "michael.johnson@example.com");

        if (john != null && jane != null && michael != null)
        {
            // John mentors Jane
            await context.UserMentors.AddAsync(new MentorMentee
            {
                MentorId = john.Id,
                MenteeId = jane.Id
            });

            // Jane mentors Michael
            await context.UserMentors.AddAsync(new MentorMentee
            {
                MentorId = jane.Id,
                MenteeId = michael.Id
            });

            await context.SaveChangesAsync();
        }
    }
}