using Domain.Users.Entities;
using IntegrationsTests.Seed;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Database.Seed.Users;

internal static class SeedData
{
    public static async Task Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expertise>().HasData(LanguageExpertiseSeeder.Expertises());
        modelBuilder.Entity<Language>().HasData(LanguageExpertiseSeeder.Languages());

    }

}