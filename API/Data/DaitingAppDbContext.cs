using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DaitingAppDbContext : DbContext
{
    public DaitingAppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; } 
}
