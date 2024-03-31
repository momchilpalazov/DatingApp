

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;

namespace API.Data
{
    public class SeedData
    {
        
        public static async Task SeedUsers(DaitingAppDbContext context)
        {
            if (!context.Users.Any())
            {
                var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);
                foreach (var user in users)
                {
                    using var hmac = new HMACSHA512();

                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                    user.PasswordSalt = hmac.Key;
                    context.Users.Add(user);
                }
                await context.SaveChangesAsync();
            }
        }

        
    }
}