using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{

public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DaitingAppDbContext>(options => 
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection")); 
        });

        services.AddCors();
        services.AddScoped<ITokenService,TokenService>();
       
        return services;      



    }
    
}
