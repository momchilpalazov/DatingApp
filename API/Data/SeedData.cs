using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class SeedData
    {

        public static async Task ClearConections(DaitingAppDbContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();      
        }
        
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            
                var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");
             
                var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);

                var roles= new List<AppRole>
                {
                    new AppRole{Name="Member"},
                    new AppRole{Name="Admin"},
                    new AppRole{Name="Moderator"}
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                
                foreach (var user in users)
                {                   

                    user.UserName = user.UserName.ToLower();

                    if (user.Created != null && user.Created.Kind == DateTimeKind.Unspecified)
                        {
                            user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                        }

                        if (user.LastActive != null && user.LastActive.Kind == DateTimeKind.Unspecified)
                        {
                            user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
                        }                    
                                       
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");

                }

                var admin=new AppUser
                {
                    UserName="admin"
                };


                await userManager.CreateAsync(admin, "Pa$$w0rd");    

                await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});      
                
            
        }

        
    }
}