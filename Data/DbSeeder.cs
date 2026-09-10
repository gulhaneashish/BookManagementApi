using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        BookStoreDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "Admin@123"),
            Role = "Admin"
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}