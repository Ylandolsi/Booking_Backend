using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Database.Seed.Users;

internal static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expertise>().HasData(DataForSeeding.Expertises());
        modelBuilder.Entity<Language>().HasData(DataForSeeding.Languages());
    }

}