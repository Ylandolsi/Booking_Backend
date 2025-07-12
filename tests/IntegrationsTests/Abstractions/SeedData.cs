using Application.Abstractions.Data;
using Domain.Users.Entities;
using Infrastructure.Database.Seed.Users;
using Microsoft.EntityFrameworkCore;

internal static class SeedData
{
    public static async Task Initialize(IApplicationDbContext context)
    {
        try
        {
            if (!await context.Languages.AnyAsync())
            {
                await context.Languages.AddRangeAsync(DataForSeeding.Languages());
                Console.WriteLine($"Added languages");
            }

            if (!await context.Expertises.AnyAsync())
            {
                await context.Expertises.AddRangeAsync(DataForSeeding.Expertises());
                Console.WriteLine($"Added expertises");
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Seed data saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}