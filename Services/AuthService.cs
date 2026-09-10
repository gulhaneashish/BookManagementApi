using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services;

public class AuthService
{
    private readonly BookStoreDbContext _context;

    public AuthService(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(
     string username,
     string password)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (existingUser != null)
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(password),

            Role = "User"
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> ValidateUserAsync(
        string username,
        string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return null;
        }

        var validPassword =
            BCrypt.Net.BCrypt.Verify(
                password,
                user.PasswordHash);

        if (!validPassword)
        {
            return null;
        }

        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByRefreshTokenAsync(
    string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken);
    }
}