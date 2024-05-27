using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Repository;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
namespace API.Extensions;
public static class ApplicationServiceExtensions
{

public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DaitingAppDbContext>(options => 
        {
            
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));

        });

        services.AddCors();
        services.AddScoped<ITokenService,TokenService>();  
        // services.AddScoped<IUserRepository,UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());  
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService,PhotoService>();
        services.AddScoped<LogUserActivity>();
        // services.AddScoped<IlikeRepository,LikeRepository>();
        // services.AddScoped<IMessageRepository,MessageRepository>();
        services.AddSignalR();
       services.AddSingleton<PresentTracker>();
        services.AddScoped<IUnitOfWorkRepository,UnitOfWork>();

       
        return services;      


    }
    
}
