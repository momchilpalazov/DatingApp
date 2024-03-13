using API.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DaitingAppDbContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddCors(

    options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");

        });
    }
);


var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

//app.UseAuthorization();
app.MapControllers();
app.Run();
