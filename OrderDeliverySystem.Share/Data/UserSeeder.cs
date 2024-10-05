using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.Data
{
    public class UserSeeder
    {
        public static async Task EnsureAdminUserExists(AppDbContext context)
        {
            const string Role = "Admin";
            const string Email = "admin@123.com";
            const string Password = "Admin@123";

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (existingUser == null)
            {
                var newUser = new User
                {
                    Email = Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                    Role = Role,
                    IsActived = 1,
                    CreatedAt = DateTime.Now,
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}
